//
// Mono.ILASM.FieldInstr
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//




namespace Mono.ILASM {

        public class FieldInstr : IInstr {

                readonly private PEAPI.FieldOp op;
                readonly private IFieldRef operand;

                public FieldInstr (PEAPI.FieldOp op, IFieldRef operand, Location loc)
			: base (loc)
                {
                        this.op = op;
                        this.operand = operand;
                }

                public override void Emit (CodeGen code_gen, MethodDef meth,
					   PEAPI.CILInstructions cil)
                {
                        operand.Resolve (code_gen);
                        cil.FieldInst (op, operand.PeapiField);
                }

        }

}



