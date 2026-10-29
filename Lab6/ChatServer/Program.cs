using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



int port = 5555;

if (args.Length > 0)
{
    if (!int.TryParse(args[0], out port) || port < 500 || port > 6500)
    {
        Console.WriteLine(
            "Ошибка: укажите порт от 500 до 6500.");

        return;
    }
}

const string LogFile = "chat.log";
var logLock = new SemaphoreSlim(1, 1);


var clients = new List<ChatUser>();
var lockObj = new object();

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine($"Сервер запущен на порту {port}. Ctrl+C — остановка.");

while (true)
{
    TcpClient client = await listener.AcceptTcpClientAsync();
    _ = HandleClientAsync(client);              // каждого клиента — в свою задачу
}

async Task HandleClientAsync(TcpClient client)
{
    var endpoint = client.Client.RemoteEndPoint;
    var stream = client.GetStream();
    var reader = new StreamReader(stream);
    var writer = new StreamWriter(stream) { AutoFlush = true };


    string? nick;

    while (true)
    {
        nick = await reader.ReadLineAsync();

        if (string.IsNullOrEmpty(nick))
        {
            await writer.WriteLineAsync("*** Пустые ники запрещены! ***");
            await writer.WriteLineAsync("Введите ник");
            continue;
        }

        bool nickExist = false;
        lock (lockObj) // проверка на уже существующий ник
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (nick == clients[i].Nick)
                {
                    nickExist = true;
                }
            }
        }
        if (nickExist)
        {
            await writer.WriteLineAsync($"Ник: {nick} уже используется, выберите другой ник");
            continue;
        }
        break;
    }


    var user = new ChatUser(nick, writer);
    
    lock (lockObj) clients.Add(user);
    string connectMessage = $"[{DateTime.Now:HH:mm:ss}] {nick} подключился ({endpoint})";

    Console.WriteLine(connectMessage);
    await WriteLogAsync(connectMessage);

    await writer.WriteLineAsync("Подключено. Пишите сообщения, /exit — выход.");
    await BroadcastAsync($"*** {nick} вошёл в чат ***");

    try
    {
        string? line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            string[] parts = line.Split(new[] { ' ' },3,StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                continue;

            switch (parts[0])
            {
                case "/exit":
                    return;

                case "/list":
                    await GetAllUsers(user);
                    break;

                case "/help":
                    await writer.WriteLineAsync(
                        "/exit - выход\n" +
                        "/list - список всех пользователей\n" +
                        "/w <ник> <сообщение> - личное сообщение\n" +
                        "/help - подсказки");
                    break;

                case "/w":
                    if (parts.Length < 3)
                    {
                        await writer.WriteLineAsync(
                            "Использование: /w <ник> <сообщение>");
                        break;
                    }

                    string receiverNick = parts[1];
                    string privateMessage = parts[2];

                    await PrivateMsg(
                        user,
                        privateMessage,
                        receiverNick);

                    break;

                default:
                    string publicMessage = $"[{DateTime.Now:HH:mm:ss}] {nick}: {line}";

                    await WriteLogAsync(publicMessage);
                    await BroadcastAsync(publicMessage);
                    break;
            }
        }
    }
    catch (IOException) { /* клиент оборвал соединение */ }
    finally
    {
        lock (lockObj) clients.Remove(user);
        client.Close();
        string disconnectMessage =$"[{DateTime.Now:HH:mm:ss}] {nick} отключился";

        Console.WriteLine(disconnectMessage);
        await WriteLogAsync(disconnectMessage);
        await BroadcastAsync($"*** {nick} покинул чат ***");
    }
}

async Task WriteLogAsync(string message)
{
    string logLine =
        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

    await logLock.WaitAsync();

    try
    {
        using (var writer = new StreamWriter(LogFile, append: true, encoding: Encoding.UTF8))
        {
            await writer.WriteLineAsync(logLine);
        }
    }
    finally
    {
        logLock.Release();
    }
}

async Task GetAllUsers(ChatUser chatUser)
{
    List<ChatUser> snapshot;

    lock (lockObj)
    {
        snapshot = new List<ChatUser>(clients);
    }

    await chatUser.Writer.WriteLineAsync("Эти пользователи сейчас в сети ->");
    var users = new StringBuilder();
    for(int i  = 0; i < snapshot.Count; i++)
    {
        users.AppendLine(snapshot[i].Nick);
    }
    await chatUser.Writer.WriteLineAsync(users.ToString());

}

async Task PrivateMsg(ChatUser sender, string message, string receiverNick)
{
    foreach (var client in clients)
    {
        if (client.Nick == receiverNick)
        {
            await client.Writer.WriteLineAsync(
                $"[{DateTime.Now:HH:mm:ss}] Личное сообщение от {sender.Nick}: {message}");

            await sender.Writer.WriteLineAsync($"[{DateTime.Now:HH:mm:ss}] Вы отправили сообщение пользователю {receiverNick}: {message}");

            return;
        }
    }

    //если пользователь не найден
    await sender.Writer.WriteLineAsync($"Пользователь с ником {receiverNick} не найден. Сообщение не доставлено.");
}


async Task BroadcastAsync(string message)
{
    List<ChatUser> snapshot;
    lock (lockObj)
    {
        snapshot = new List<ChatUser>(clients);  // копия под блокировкой
    }
    foreach (var client in snapshot)
    {
        try { await client.Writer.WriteLineAsync(message); }
        catch (IOException) { /* отвалившийся клиент удалится в своей задаче */ }
    }
}

public class ChatUser
{
    public string Nick { get; set; }
    public StreamWriter Writer { get; set; }


    public ChatUser(string nick, StreamWriter writer)
    {
        Nick = nick; 
        Writer = writer;
    }
}