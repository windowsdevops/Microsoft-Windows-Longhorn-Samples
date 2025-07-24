Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Data

Namespace Get_Set_VB

    '@ <summary>
    '@ Interaction logic for Pane1_xaml.xaml
    '@ </summary>
    Expands Class Pane1
    Sub btnGoTo2(ByVal Sender As Object, ByVal args As System.Windows.Controls.ClickEventArgs)
      Dim myApp As System.Windows.Navigation.NavigationApplication
      Dim navWindow As System.Windows.Navigation.NavigationWindow
      Dim nextUri As System.Uri

      myApp = CType(System.Windows.Application.Current, System.Windows.Navigation.NavigationApplication)
      navWindow = CType(myApp.MainWindow, System.Windows.Navigation.NavigationWindow)
      myApp.Properties("TextFromPage1") = txtBox.Text
      nextUri = New System.Uri("Pane2.xaml", False, True)
      navWindow.Navigate(nextUri)
    End Sub 'btnGoTo2

    End Class

End Namespace