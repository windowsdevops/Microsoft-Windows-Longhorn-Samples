#Const Debug = True  'Workaround - This allows Debug.Write to work.

' This is a list of commonly used namespaces for an application class.
Imports System
Imports System.Windows
Imports System.Data
Imports System.Xml
Imports System.Configuration

Namespace EnableAndDisableTextFormattingNew

    '@ <summary>
    '@ Interaction logic for Application.xaml
    '@ </summary>
    Expands Class MyApp

        Sub AppStartingUp(ByVal sender As Object, ByVal e As StartingUpCancelEventArgs)
            Dim mainWindow As Window = New Window1
            mainWindow.Show()
        End Sub

    End Class

End Namespace