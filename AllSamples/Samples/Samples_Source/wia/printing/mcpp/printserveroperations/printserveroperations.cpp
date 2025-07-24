//---------------------------------------------------------------------
//  This file is part of the Microsoft .NET Framework SDK Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//This source code is intended only as a supplement to Microsoft
//Development Tools and/or on-line documentation.  See these other
//materials for detailed information regarding Microsoft code samples.
//
//THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//PARTICULAR PURPOSE.
//---------------------------------------------------------------------

#include "stdafx.h"

#using <mscorlib.dll>

using namespace System;
using namespace System::IO;
using namespace System::Collections;

using namespace System::Printing;
using namespace System::Printing::PrintSubSystem;
using namespace System::Printing::Configuration;
using namespace Microsoft::Printing::JobTicket;

#ifndef  __PRINTSERVERSAMPLE_HPP__
#include "PrintServerSample.hpp"
#endif

using namespace SystemPrintingSample;

[STAThread]

void
WriteCommandArguments(
    void
    )
{
    Console::WriteLine(S"Incomplete command line arguments");
	Console::WriteLine("Enter:"
		               "PrintServerOperations <PrintServerName> <New Spool Directory name> ");
	Console::WriteLine("This would run with the following defaults:");
	Console::WriteLine("BeepEnabled: true");
	Console::WriteLine("To change the defaults edit the application and compile");
}

void _tmain(int argc, char* argv[])
{
    try
    {
		Console::WriteLine("System.Printing Print Server Query / Set sample ...");
        if(argc<3)
        {
            WriteCommandArguments();
        }
		else
		{
	        PrintServerSample::PrintServerOperations(argv[1],argv[2]);
		}
    }
    catch(PrintQueueException* e)
    {
        String* message = e->Message;
    }

	Console::ReadLine();
}
