//
// Mono.ILASM.SwitchInstr
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//


using System.Collections;

namespace Mono.ILASM
{

    public class SwitchInstr : IInstr
    {

        readonly private ArrayList label_list;

        public SwitchInstr(ArrayList label_list, Location loc)
    : base(loc)
        {
            this.label_list = label_list;
        }

        public override void Emit(CodeGen code_gen, MethodDef meth,
               PEAPI.CILInstructions cil)
        {
            int count = 0;
            PEAPI.CILLabel[] label_array;

            if (label_list != null)
            {
                label_array = new PEAPI.CILLabel[label_list.Count];
                foreach (object lab in label_list)
                {
                    if (lab is LabelInfo info)
                    {
                        label_array[count++] = info.Label;
                    }
                    else
                    {
                        throw new InternalErrorException("offsets in switch statements.");
                    }
                }
            }
            else
            {
                label_array = [];
            }

            cil.Switch(label_array);
        }
    }

}

