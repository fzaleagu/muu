Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Muu

<TestClass()> Public Class RequestBuilderTest

    <TestMethod()> Public Sub RequestIsNotCompleteWhenEmpty()
        Dim request = New RequestBuilder()
        Assert.IsFalse(request.IsComplete())
    End Sub

    <TestMethod()> Public Sub RequestIsNotCompleteWithOneElement()
        Dim request = New RequestBuilder()
        request.AppendData(New Byte() {13})
        Assert.IsFalse(request.IsComplete())
    End Sub

    <TestMethod()> Public Sub RequestIsCompleteWhenOnlyCRLF()
        Dim request = New RequestBuilder()
        request.AppendData(New Byte() {13, 10, 13, 10})
        Assert.IsTrue(request.IsComplete())
    End Sub

    <TestMethod()> Public Sub RequestIsCompleteWhenContentAndCRLF()
        Dim request = New RequestBuilder()
        request.AppendData(New Byte() {10, 20, 30, 40, 50, 13, 10, 13, 10})
        Assert.IsTrue(request.IsComplete())
    End Sub

    <TestMethod()> Public Sub RequestIsNotCompleteWhenContentHasCRL()
        Dim request = New RequestBuilder()
        request.AppendData(New Byte() {10, 20, 30, 40, 50, 13, 10, 13})
        Assert.IsFalse(request.IsComplete())
    End Sub

    <TestMethod()> Public Sub RequestIsNotCompleteWhenContentHasCR()
        Dim request = New RequestBuilder()
        request.AppendData(New Byte() {10, 20, 30, 40, 50, 13, 10})
        Assert.IsFalse(request.IsComplete())
    End Sub

    <TestMethod()> Public Sub RequestIsCompleteWhenContentHasCRLFAndMore()
        Dim request = New RequestBuilder()
        request.AppendData(New Byte() {10, 20, 30, 40, 50, 13, 10, 13, 10, 10, 20, 30, 40})
        Assert.IsTrue(request.IsComplete())
    End Sub

End Class