﻿Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Muu

<TestClass()> Public Class RequestTest

    <TestMethod()>
    Public Sub RequestHasCorrectMethod()
        Dim str = "GET /index.html HTTP/1.1" + ControlChars.CrLf
        Dim data = Encoding.ASCII.GetBytes(str)
        Dim request = New Request(data)
        Assert.AreEqual("GET", request.Method)
    End Sub

    <TestMethod()>
    Public Sub RequestHasCorrectURI()
        Dim str = "GET /index.html HTTP/1.1" + ControlChars.CrLf
        Dim data = Encoding.ASCII.GetBytes(str)
        Dim request = New Request(data)
        Assert.AreEqual("/index.html", request.RequestURI)
    End Sub

    <TestMethod()>
    Public Sub RequestHasCorrectHttpVersion()
        Dim str = "GET /index.html HTTP/1.1" + ControlChars.CrLf
        Dim data = Encoding.ASCII.GetBytes(str)
        Dim request = New Request(data)
        Assert.AreEqual("HTTP/1.1", request.HttpVersion)
    End Sub

    <TestMethod()>
    Public Sub RequestHasCorrectFileName()
        Dim str = "GET /index.html HTTP/1.1" + ControlChars.CrLf
        Dim data = Encoding.ASCII.GetBytes(str)
        Dim request = New Request(data)
        Assert.AreEqual("index.html", request.FileName)
    End Sub

End Class
