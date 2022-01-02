//
// DebuggingInfo.cs
//
// Author(s):
//  Martin Baulig (martin@ximian.com)
//
// Copyright (C) 2004 Novell, Inc.
//

using System;
using System.Collections;
using Mono.CompilerServices.SymbolWriter;

namespace Mono.ILASM
{

    public class SymbolWriter : MonoSymbolWriter
    {
        SourceMethod current_method;
        CompileUnitEntry current_source;
        readonly ArrayList methods;

        public SymbolWriter(string filename)
            : base(filename)
        {
            methods = new ArrayList();
        }

        public SourceMethod BeginMethod(MethodDef method, Location start)
        {
            current_method = new SourceMethod(current_source, method, start);
            methods.Add(current_method);
            return current_method;
        }

        public void EndMethod(Location end)
        {
            current_method.EndLine = end.line;
            current_method = null;
        }

        public void BeginSourceFile(string filename)
        {
            SourceFileEntry file = DefineDocument(filename, null, null);
            current_source = DefineCompilationUnit(file);
        }

        public void EndSourceFile()
        {
            current_source = null;
        }

        public void Write(Guid guid)
        {
            foreach (SourceMethod method in methods)
                method.Write(this);

            WriteSymbolFile(guid);
        }
    }

    public class SourceMethod : IMethodDef
    {
        readonly CompileUnitEntry file;
        readonly MethodDef method;
        readonly ArrayList lines;
        public int StartLine, EndLine;

        public SourceMethod(CompileUnitEntry file, MethodDef method, Location start)
        {
            this.file = file;
            this.method = method;
            this.StartLine = start.line;

            lines = new ArrayList();
            MarkLocation(start.line, 0);
        }

        public string Name
        {
            get { return method.Name; }
        }

        public int Token
        {
            get
            {
                PEAPI.MethodDef pemethod = method.PeapiMethodDef;
                return (int)(((uint)PEAPI.MDTable.Method << 24) | pemethod.Row);
            }
        }

        public void MarkLocation(int line, uint offset)
        {
            lines.Add(new LineNumberEntry(0, line, (int)offset));
        }

        public void Write(MonoSymbolWriter writer)
        {
            LineNumberEntry[] the_lines = new LineNumberEntry[lines.Count];
            lines.CopyTo(the_lines, 0);

            LocalVariableEntry[] locals = method.GetLocalVars();
            _ = writer.SymbolFile.DefineMethod(
                file, Token, null, locals, the_lines, null, null, 0, 0);
        }
    }
}
