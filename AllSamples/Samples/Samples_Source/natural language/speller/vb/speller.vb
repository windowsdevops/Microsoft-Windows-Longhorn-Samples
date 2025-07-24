'---------------------------------------------------------------------
'  This file is part of the Microsoft .NET Framework SDK Code Samples.
' 
'  Copyright (C) Microsoft Corporation.  All rights reserved.
' 
'This source code is intended only as a supplement to Microsoft
'Development Tools and/or on-line documentation.  See these other
'materials for detailed information regarding Microsoft code samples.
' 
'THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
'KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
'IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
'PARTICULAR PURPOSE.
'---------------------------------------------------------------------
Imports System
Imports System.Collections
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.NaturalLanguage


Namespace Microsoft.Samples.NaturalLanguage
   Public Class SpellerService
      Public Sub New(opts As CmdOpts)
         Dim context As New Context()
         
         ' Set the following context properties to false
         context.IsSpellChecking = True
         context.IsCheckingRepeatedWords = True
         context.IsComputingInflections = False
         context.IsShowingCharacterNormalizations = False
         context.IsShowingGaps = False
         context.IsShowingWordNormalizations = False
         context.IsSingleLanguage = False
         context.IsSpellAlwaysSuggesting = False
         context.IsSpellIgnoringAllUpperCase = False
         context.IsSpellIgnoringWordsWithNumbers = False
         context.IsSpellPreReform = False
         context.IsSpellRequiringAccentedCapitals = False
         context.IsComputingLemmas = False
         context.IsFindingDateTimeMeasures = False
         context.IsFindingLocations = False
         context.IsFindingOrganizations = False
         context.IsFindingPersons = False
         context.IsFindingPhrases = False
         context.IsComputingCompounds = False
         If opts.LexiconList.Count > 0 Then
            context.Lexicons = opts.LexiconList
         End If 
         m_TextChunk = New TextChunk(context)
         m_TextChunk.Culture = opts.Culture
         m_fInteractiveMode = False
         m_iNumFiles = 0
         m_iNumSegments = 0
         m_iNumSentences = 0
         m_SpellingErrors = New SortedList()
      End Sub 'New
      
      Overloads Public Shared Function Main(args() As String) As Integer
         Dim retCode As Integer = 0
         Dim opts As CmdOpts = Nothing
         Dim ss As SpellerService = Nothing
         Dim outputWriter As TextWriter = Nothing
         Dim inputReader As TextReader = Nothing
         Dim input As String = ""
         
         Try
            opts = New CmdOpts()
            If opts.ProcessOpts(args) Then
               ss = New SpellerService(opts)
               If opts.Output.Length <> 0 Then
                  If File.Exists(opts.Output) Then
                     File.Delete(opts.Output)
                  End If 
                  outputWriter = New StreamWriter(opts.Output)
               Else
                  outputWriter = New StreamWriter(Console.OpenStandardOutput())
               End If
               
               If opts.Input.Length <> 0 Then
                  ss.Parse(opts.Input, outputWriter)
               Else
                  ss.InteractiveMode = True
                  ss.PrintBanner(outputWriter)
                  While ss.InteractiveMode
                     outputWriter.Write("nlg> ")
                     outputWriter.Flush()
                     input = Console.ReadLine()
                     If [String].Compare(input, "stat", True, CultureInfo.InvariantCulture) = 0 Then
                        ss.ShowStatistics(outputWriter)
                     Else
                        If [String].Compare(input, "quit", True, CultureInfo.InvariantCulture) = 0 Then
                           outputWriter.WriteLine("Goodbye!!")
                           ss.InteractiveMode = False
                        Else
                           If [String].Compare(input, "help", True, CultureInfo.InvariantCulture) = 0 Then
                              ss.PrintHelp(outputWriter)
                           Else
                              If [String].Compare(input, "clear", True, CultureInfo.InvariantCulture) = 0 Then
                                 ss.ClearStatistics()
                              Else
                                 inputReader = New StringReader(input)
                                 ss.Parse(inputReader, outputWriter)
                              End If
                           End If
                        End If
                     End If
                  End While
               End If
            End If
            
            retCode = 0
         Catch nre As NullReferenceException
            If opts.Verbose Then
               Console.Error.WriteLine("Message: {0}", nre.Message)
               Console.Error.WriteLine("Stack Trace: {0}", nre.StackTrace)
            End If
            
            retCode = 1
         Catch e As ArgumentException
            If opts.Verbose Then
               Console.Error.WriteLine("Message: {0}", e.Message)
               Console.Error.WriteLine("Stack Trace: {0}", e.StackTrace)
            End If
            
            retCode = 1
         Finally
            If Not (outputWriter Is Nothing) Then
               outputWriter.Close()
            End If 
            If Not (inputReader Is Nothing) Then
               inputReader.Close()
            End If
         End Try
         Return retCode
      End Function 'Main
      
      
      Public Property InteractiveMode() As Boolean
         Get
            Return m_fInteractiveMode
         End Get
         Set
            m_fInteractiveMode = value
         End Set
      End Property
       
      Public Sub PrintBanner(outputWriter As TextWriter)
         outputWriter.WriteLine("NLG Interactive Console")
         outputWriter.WriteLine("Copyright (c) 2004 Microsoft Corporation, Inc. All Rights Reserved")
         outputWriter.WriteLine("Type 'help' for more information")
         outputWriter.Flush()
      End Sub 'PrintBanner
      
      
      Public Sub ShowStatistics(outputWriter As TextWriter)
         If m_fInteractiveMode Then
            outputWriter.WriteLine("{0,-30} {1}", "Num. Text Blocks:", m_iNumFiles)
         Else
            outputWriter.WriteLine("{0,-30} {1}", "Num. Files:", m_iNumFiles)
         End If
         
         outputWriter.WriteLine("{0,-30} {1}", "Num. Sentences:", m_iNumSentences)
         outputWriter.WriteLine("{0,-30} {1}", "Num. Segments:", m_iNumSegments)
         outputWriter.WriteLine()
         outputWriter.Flush()
      End Sub 'ShowStatistics
      
      
      Public Sub PrintHelp(outputWriter As TextWriter)
         outputWriter.WriteLine("Text entered at the prompt will be analyzed by the NLG Engine")
         outputWriter.WriteLine("The following commands are supported in interactive console mode:")
         outputWriter.WriteLine("  STAT       Shows session statistics")
         outputWriter.WriteLine("  QUIT       Exits from interactive mode")
         outputWriter.WriteLine("  HELP       Prints this help message")
         outputWriter.WriteLine("  CLEAR      Clears session statistics")
         outputWriter.Flush()
      End Sub 'PrintHelp
      
      
      Public Sub ClearStatistics()
         m_SpellingErrors.Clear()
         m_iNumFiles = 0
         m_iNumSentences = 0
         m_iNumSegments = 0
      End Sub 'ClearStatistics
      
      
      Overloads Public Sub Parse(inputPath As String, outputWriter As TextWriter)
         Dim inputReader As StreamReader = Nothing
         
         If outputWriter Is Nothing Then
            Return
         End If 
         Try
            If inputPath.Length <> 0 Then
               If File.Exists(inputPath) Then
                  Dim f As New FileInfo(inputPath)
                  Dim fattrib As FileAttributes = f.Attributes
                  
                  'Skip empty files, hidden files, system files and temporary files
                  If f.Length > 0 And fattrib <>(fattrib Or FileAttributes.Hidden) And fattrib <>(fattrib Or FileAttributes.System) And fattrib <>(fattrib Or FileAttributes.Temporary) Then
                     inputReader = New StreamReader(inputPath)
                  End If
               Else
                  If Directory.Exists(inputPath) Then
                     parseDir(inputPath, outputWriter)
                  Else
                     Throw New ArgumentException("Specified Input File does not exist")
                  End If
               End If
            Else
               inputReader = New StreamReader(Console.OpenStandardInput())
            End If
            
            If Not (inputReader Is Nothing) Then
               Me.Parse(inputReader, outputWriter)
            End If
         Catch ae As ArgumentException
            Throw ae
         Finally
            If Not (inputReader Is Nothing) Then
               inputReader.Close()
            End If
         End Try
      End Sub 'Parse
       
      Overloads Public Sub Parse(inputReader As TextReader, outputWriter As TextWriter)
         m_iNumFiles += 1
         
         Dim input As String = inputReader.ReadToEnd()
         
         m_TextChunk.InputText = input
         
         Dim numMisspelled As Integer = 0
         
         Dim sentence As Sentence
         For Each sentence In  m_TextChunk
            m_iNumSentences += 1
            Dim segment As Segment
            For Each segment In  sentence
               m_iNumSegments += 1
               If emitSegmentInfo(outputWriter, segment) Then
                  numMisspelled += 1
               End If
            Next segment
         Next sentence 
         If numMisspelled = 0 Then
            outputWriter.WriteLine("<No spelling errors found>")
         End If
      End Sub 'Parse 
      Private m_TextChunk As TextChunk
      
      Private m_SpellingErrors As SortedList
      
      
      Private Sub parseDir(inputDir As String, outputWriter As TextWriter)
         ' Process the list of files found in the target directory
         Dim fileEntries As String() = Directory.GetFiles(inputDir)
         
         Dim i As Integer
         For i = 0 To fileEntries.Length - 1
            Me.Parse(fileEntries(i), outputWriter)
         Next i
         
         ' Recurse into subdirectories of the target directory
         Dim subdirectoryEntries As String() = Directory.GetDirectories(inputDir)
         
         For i = 0 To subdirectoryEntries.Length - 1
            parseDir(subdirectoryEntries(i), outputWriter)
         Next i
      End Sub 'parseDir
      
      
      Private Function emitSegmentInfo(outputWriter As TextWriter, segment As Segment) As Boolean
         Dim retValue As Boolean = False
         
         If segment.Role = RangeRole.Incorrect Or segment.Role = RangeRole.AutoReplaceForm Or segment.Role = RangeRole.Repeated Then
            outputWriter.WriteLine("{0} ({1})", segment.ToString(), segment.Role.ToString())
            outputWriter.WriteLine(" [Suggestions: {0}]", segment.Suggestions.Count.ToString(NumberFormatInfo.InvariantInfo))
            Dim suggestion As Segment
            For Each suggestion In  segment.Suggestions
               outputWriter.WriteLine("   {0} - {1}", suggestion.ToString(), suggestion.SpellingScore.ToString(NumberFormatInfo.InvariantInfo))
            Next suggestion
            
            retValue = True
         End If
         
         outputWriter.Flush()
         Return retValue
      End Function 'emitSegmentInfo
      
      Private m_iNumFiles As Integer
      
      Private m_iNumSegments As Integer
      
      Private m_iNumSentences As Integer
      
      Private m_fInteractiveMode As Boolean
   End Class ' class SpellerService
End Namespace 'Microsoft.Samples.NaturalLanguage