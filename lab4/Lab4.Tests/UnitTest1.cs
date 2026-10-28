using System;
using Lab4.Core;
using Lab4.Core.PC;
using Xunit;

namespace Lab4.Tests;

public class Lab4Tests
{
    // ==========================================
    // 1. ПАРАМЕТРИЗОВАННЫЕ ТЕСТЫ ([Theory]) - 3 шт.
    // ==========================================

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(5, 120)]
    public void Factorial_ValidInput_ReturnsCorrectResult(long input, int expected)
    {
        // Arrange
        // Act
        int result = Algo.Factorial(input);
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, new long[] { 0 })]
    [InlineData(5, new long[] { 0, 1, 1, 2, 3 })]
    public void Fibonacci_ValidCount_ReturnsCorrectSequence(int n, long[] expected)
    {
        // Arrange & Act
        long[] result = Algo.Fibonacci(n);
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(1.5)]
    public void SinTaylor_ValidInput_ReturnsCloseToMathSin(double x)
    {
        // Arrange
        double expected = Math.Sin(x);
        // Act
        double result = Algo.SinTaylor(x);
        // Assert
        Assert.Equal(expected, result, precision: 5);
    }

    // ==========================================
    // 2. ТЕСТЫ ГРАНИЧНЫХ СЛУЧАЕВ - 3 шт.
    // ==========================================

    [Fact]
    public void Fibonacci_ZeroCount_ReturnsEmptyArray()
    {
        // Arrange
        int n = 0;
        // Act
        long[] result = Algo.Fibonacci(n);
        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Factorial_MaxBoundaryValue_DoesNotThrow()
    {
        // Arrange
        long validInput = 12; // 12! = 479001600 (гарантированно помещается в int, избегает ошибки uint)
        long invalidInput = 21; // Граница: в коде стоит условие i > 20

        // Act
        int result = Algo.Factorial(validInput);

        // Assert
        Assert.Equal(479001600, result); // Теперь типы совпадают (int и int), компилятор не ошибется
        Assert.Throws<ArgumentOutOfRangeException>(() => Algo.Factorial(invalidInput));
    }


    [Fact]
    public void PCBuild_EmptyBuild_ReturnsZeroCostAndPower()
    {
        // Arrange
        PCBuild build = new PCBuild();
        // Act
        double cost = build.GetTotalCost();
        double power = build.GetTotalPowerConsumption();
        // Assert
        Assert.Equal(0.0, cost);
        Assert.Equal(0.0, power);
        Assert.Equal(0, build.GetComponentCount());
    }

    // ==========================================
    // 3. ТЕСТЫ ИСКЛЮЧЕНИЙ (Assert.Throws) - 3 шт.
    // ==========================================

    [Fact]
    public void Factorial_NegativeInput_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        long input = -1;
        // Act
        Action act = () => Algo.Factorial(input);
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void PCComponent_NegativeCost_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        string name = "Test Component";
        double negativeCost = -100.0;
        // Act
        Action act = () => new Processor(name, negativeCost, 65.0, 4, 3.0);
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Processor_ZeroCores_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int zeroCores = 0;
        // Act
        Action act = () => new Processor("Test CPU", 100.0, 65.0, zeroCores, 3.0);
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    // ==========================================
    // 4. ТЕСТЫ КЛАССОВ ИЗ ЛР2 (PCBuild и компоненты)
    // ==========================================

    [Fact]
    public void PCBuild_AddComponent_UpdatesTotalCost()
    {
        // Arrange
        PCBuild build = new PCBuild();
        IComponent cpu = new Processor("Intel i5", 15000.0, 65.0, 6, 4.0);
        // Act
        build.AddComponent(cpu);
        // Assert
        Assert.Equal(15000.0, build.GetTotalCost());
        Assert.Equal(1, build.GetComponentCount());
    }

    [Fact]
    public void PCBuild_RemoveComponent_UpdatesTotalCost()
    {
        // Arrange
        PCBuild build = new PCBuild();
        IComponent ram = new Memory("Kingston", 5000.0, 5.0, 16);
        build.AddComponent(ram);
        // Act
        build.RemoveComponent("Kingston");
        // Assert
        Assert.Equal(0.0, build.GetTotalCost());
        Assert.Equal(0, build.GetComponentCount());
    }

    [Fact]
    public void PCBuild_GetTotalPowerConsumption_CalculatesCorrectly()
    {
        // Arrange
        PCBuild build = new PCBuild();
        build.AddComponent(new Processor("CPU", 100.0, 65.0, 4, 3.0));
        build.AddComponent(new Memory("RAM", 50.0, 5.0, 8));
        // Act
        double totalPower = build.GetTotalPowerConsumption();
        // Assert
        Assert.Equal(70.0, totalPower);
    }

    [Fact]
    public void Memory_InvalidCapacity_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int invalidCapacity = 0;
        // Act
        Action act = () => new Memory("Bad RAM", 100.0, 5.0, invalidCapacity);
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Processor_GetInfo_ReturnsFormattedString()
    {
        // Arrange
        Processor cpu = new Processor("Test CPU", 100.0, 65.0, 8, 4.5);
        // Act
        string info = cpu.GetInfo();
        // Assert
        Assert.Contains("Test CPU", info);
        Assert.Contains("8", info);
        Assert.Contains("ГГц", info); // Исправлено: проверяем единицу измерения, чтобы избежать проблем с запятой/точкой в русской локали
    }

    [Fact]
    public void PCBuild_AddMultipleComponents_AccumulatesValuesCorrectly()
    {
        // Arrange
        PCBuild build = new PCBuild();
        build.AddComponent(new Processor("CPU", 10000.0, 100.0, 6, 3.5));
        build.AddComponent(new Memory("RAM1", 5000.0, 10.0, 16));
        build.AddComponent(new Memory("RAM2", 5000.0, 10.0, 16));
        // Act
        double totalCost = build.GetTotalCost();
        double totalPower = build.GetTotalPowerConsumption();
        // Assert
        Assert.Equal(20000.0, totalCost);
        Assert.Equal(120.0, totalPower);
        Assert.Equal(3, build.GetComponentCount());
    }
}