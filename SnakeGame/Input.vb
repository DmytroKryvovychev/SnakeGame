Public Class Input
    Private Shared keyTable As New Hashtable()

    Public Overloads Shared Function Keypressed(ByVal key As Key) As Boolean
        If keyTable(key) = vbNull Then
            Return False
        End If
        Return keyTable(key) = True
    End Function

    Public Shared Sub ChangeState(ByVal key As Key, ByVal state As Boolean)
        keyTable(key) = state
    End Sub
End Class
