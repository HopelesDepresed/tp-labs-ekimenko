using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

const int N = 30_000;

var swTotal = Stopwatch.StartNew();

// --- 1. Генерация данных ---
var rnd = new Random(42);                       // фиксированное зерно: результат воспроизводим
var buyers = new List<string>();
var amounts = new List<int>();
for (int i = 0; i < N; i++)
{
    buyers.Add("user" + rnd.Next(0, 5000));
    amounts.Add(rnd.Next(1, 10_000));
}

// --- 2. Список уникальных покупателей ---
var sw = Stopwatch.StartNew();
var unique = new HashSet<string>();
for (int i = 0; i < buyers.Count; i++)
{
    if (!unique.Contains(buyers[i]))
    {
        unique.Add(buyers[i]);
    }
}
Console.WriteLine($"Уникальных покупателей: {unique.Count}  ({sw.ElapsedMilliseconds} мс)");

// --- 3. Текстовый отчёт ---
sw.Restart();
StringBuilder report = new StringBuilder();
for (int i = 0; i < buyers.Count; i++)
{
    report.Append(buyers[i] + ";" + amounts[i] + Environment.NewLine);
}
Console.WriteLine($"Отчёт: {report.Length} символов  ({sw.ElapsedMilliseconds} мс)");

// --- 4. Сортировка сумм ---
sw.Restart();
var sorted = new List<int>(amounts);
sorted.Sort();
Console.WriteLine($"Медианная сумма: {sorted[sorted.Count / 2]}  ({sw.ElapsedMilliseconds} мс)");

// --- 5. Сумма покупок каждого покупателя ---
sw.Restart();
var totals = new Dictionary<string , long>();
for(int i = 0; i < buyers.Count; i++)
{
    if (!totals.ContainsKey(buyers[i]))
    {
        totals.Add(buyers[i], amounts[i]);
    }
    else
    {
        totals[buyers[i]] += amounts[i];
    }
}
var max = totals.MaxBy(t => t.Value);

Console.WriteLine(
    $"Максимум потратил: {max.Key} ({max.Value}) ({sw.ElapsedMilliseconds} мс)"
);

Console.WriteLine($"ИТОГО: {swTotal.ElapsedMilliseconds} мс");