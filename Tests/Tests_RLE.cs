using ConsoleApp1;
using FluentAssertions;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase(100500)]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(32)]
    [TestCase(64)]
    [TestCase(301)]
    [TestCase(200)]
    public void Test_Gamma(int num)
    {
        var encoded = RleDeltaEncoder.GammaEncode(num);
        var decoded = RleDeltaEncoder.GammaDecode(encoded, 0);
        decoded.number.Should().Be(num);
        decoded.bitsRead.Should().Be(encoded.Length);
    }

    [TestCase(1, "1")]
    [TestCase(2, "010")]
    [TestCase(3, "011")]
    [TestCase(4, "00100")]
    [TestCase(5, "00101")]
    public void Test_GammaBits(int num, string expectedBits)
    {
        var encoded = RleDeltaEncoder.GammaEncode(num);

        encoded.Should().Be(expectedBits);
    }

    [TestCase(1)]
    [TestCase(10)]
    [TestCase(32)]
    [TestCase(64)]
    [TestCase(301)]
    [TestCase(200)]
    [TestCase(100500)]
    public void Test_Delta(int num)
    {
        var encoded = RleDeltaEncoder.DeltaEncode(num);
        var decoded = RleDeltaEncoder.DeltaDecode(encoded, 0);
        decoded.number.Should().Be(num);
        decoded.bitsRead.Should().Be(encoded.Length);
    }

    [TestCase(1, "1")]
    [TestCase(2, "0100")]
    [TestCase(3, "0101")]
    [TestCase(4, "01100")]
    [TestCase(5, "01101")]
    [TestCase(6, "01110")]
    [TestCase(7, "01111")]
    public void Test_DeltaBits(int num, string expectedBits)
    {
        var encoded = RleDeltaEncoder.DeltaEncode(num);
        encoded.Should().Be(expectedBits);
    }
}