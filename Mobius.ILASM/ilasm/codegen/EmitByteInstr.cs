//
// Mono.ILASM.EmitByteIntr.cs
//
// Author(s):
//  Rodrigo Kumpera (rkumpera@novell.com)
//
// (C) 2007 Novell, Inc (http://www.novell.com)
//



namespace Mono.ILASM {

        public class EmitByteInstr : IInstr {

                readonly private int value;

                public EmitByteInstr (int value, Location loc)
			: base (loc)
                {
                        this.value = value;
                }

                public override void Emit (CodeGen code_gen, MethodDef meth,
					   PEAPI.CILInstructions cil)
                {
                        cil.emitbyte ((byte)value);
                }
        }

}

