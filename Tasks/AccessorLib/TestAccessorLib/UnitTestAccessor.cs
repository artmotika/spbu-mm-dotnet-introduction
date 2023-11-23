using AccessorLib;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.IO;
using System.Linq.Expressions;

namespace TestAccessorLib;

class A
{
    public int Idx { get; set; } 
}

class B
{
    public int Idx { get; set; }
    public A? ObjA { get; set; }
    public static C? ObjC { get; set; }
}

class C
{
    private int Idx { get; set; } = 80;
}

class C2
{
    private int Idx { get; set; }

    public C2(int idx)
    {
        Idx = idx;
    }
}

class D
{
    private static int Idx { get; set; } = 81;
    public static B? ObjB { get; set; }
}

[TestClass]
public class UnitTestAccessor
{
    [TestMethod]
    public void TestCreatePropertyAccessor1()
    {
        var obj = new A();
        obj.Idx = 30;
        var getProperty = Accessor.CreatePropertyAccessor<A, int>("Idx");
        int idx = getProperty(obj);   
        Assert.AreEqual(obj.Idx, idx);
    }

    [TestMethod]
    public void TestCreatePropertyAccessor2()
    {
        var objA = new A();
        objA.Idx = 30;
        var objB = new B();
        objB.ObjA = objA;
        var getProperty = Accessor.CreatePropertyAccessor<B, int>("ObjA.Idx");
        int idx = getProperty(objB);
        Assert.AreEqual(objA.Idx, idx);
    }

    [TestMethod]
    public void TestCreatePropertyAccessor3()
    {
        var objA = new A();
        objA.Idx = 30;
        var objB = new B();
        objB.ObjA = objA;
        var getProperty = Accessor.CreatePropertyAccessor<B, A>("ObjA");
        var newObj = getProperty(objB);
        Assert.AreEqual(objA, newObj);
    }

    [TestMethod]
    public void TestCreatePropertyAccessor4()
    {
        var obj = new C();
        var getProperty = Accessor.CreatePropertyAccessor<C, int>("Idx");
        int idx = getProperty(obj);
        Assert.AreEqual(80, idx);
    }

    [TestMethod]
    public void TestCreatePropertyAccessor5()
    {
        var obj = new C2(80);
        var getProperty = Accessor.CreatePropertyAccessor<C2, int>("Idx");
        int idx = getProperty(obj);
        Assert.AreEqual(80, idx);
    }

    [TestMethod]
    public void TestCreatePropertyAccessor6()
    {
        var obj = new D();
        var getProperty = Accessor.CreatePropertyAccessor<D, int>("Idx");
        int idx = getProperty(obj);
        Assert.AreEqual(81, idx);
    }

    [TestMethod]
    public void TestCreatePropertyAccessor7()
    {
        var objD = new D();
        var objB = new B();
        var objA = new A();
        objA.Idx = 30;
        objB.ObjA = objA;
        D.ObjB = objB;
        var getProperty = Accessor.CreatePropertyAccessor<D, int>("ObjB.ObjA.Idx");
        int idx = getProperty(objD);
        Assert.AreEqual(objA.Idx, idx);
    }

    [TestMethod]
    public void TestCreatePropertyAccessor8()
    {
        var objD = new D();
        var objB = new B();
        var objC = new C();
        B.ObjC = objC;
        D.ObjB = objB;
        var getProperty = Accessor.CreatePropertyAccessor<D, int>("ObjB.ObjC.Idx");
        int idx = getProperty(objD);
        Assert.AreEqual(80, idx);
    }
}