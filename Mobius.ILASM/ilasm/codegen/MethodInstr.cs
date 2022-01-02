//
// Mono.ILASM.MethodInstr
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//




namespace Mono.ILASM {

        public class MethodInstr : IInstr {

                readonly private PEAPI.MethodOp op;
                readonly private BaseMethodRef operand;

                public MethodInstr (PEAPI.MethodOp op, BaseMethodRef operand, Location loc)
			: base (loc)
                {
                        this.op = op;
                        this.operand = operand;

                        if (op == PEAPI.MethodOp.newobj || op == PEAPI.MethodOp.callvirt)
                                operand.CallConv |= PEAPI.CallConv.Instance;
                }

                public override void Emit (CodeGen code_gen, MethodDef meth,
					   PEAPI.CILInstructions cil)
                {
                        operand.Resolve (code_gen);
                        cil.MethInst (op, operand.PeapiMethod);
                }
        }

}


