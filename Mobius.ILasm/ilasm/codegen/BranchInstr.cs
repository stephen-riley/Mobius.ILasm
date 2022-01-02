//
// Mono.ILASM.BranchInstr
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//




namespace Mono.ILASM {

        public class BranchInstr : IInstr {

                readonly private PEAPI.BranchOp op;
                readonly private LabelInfo label;
	
                public BranchInstr (PEAPI.BranchOp op, LabelInfo label, Location loc)
			: base (loc)
                {
                        this.op = op;
                        this.label = label;
                }

                public override void Emit (CodeGen code_gen, MethodDef meth,
					   PEAPI.CILInstructions cil)
                {
			cil.Branch (op, label.Label);
                }
        }

}

