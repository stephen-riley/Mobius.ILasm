//
// Mono.ILASM.CalliInstr
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//

#pragma warning disable IDE0130
namespace Mono.ILASM;
#pragma warning restore IDE0130

public class CalliInstr : IInstr
{

    readonly private PEAPI.CallConv call_conv;
    readonly private BaseTypeRef ret_type;
    readonly private BaseTypeRef[] param;

    public CalliInstr(PEAPI.CallConv call_conv, BaseTypeRef ret_type,
       BaseTypeRef[] param, Location loc)
: base(loc)
    {
        this.call_conv = call_conv;
        this.ret_type = ret_type;
        this.param = param;
    }

    public override void Emit(CodeGen code_gen, MethodDef meth,
           PEAPI.CILInstructions cil)
    {
        PEAPI.Type[] param_array;
        PEAPI.CalliSig callisig;

        if (param != null)
        {
            param_array = new PEAPI.Type[param.Length];
            int count = 0;
            foreach (BaseTypeRef typeref in param)
            {
                typeref.Resolve(code_gen);
                param_array[count++] = typeref.PeapiType;
            }
        }
        else
        {
            param_array = [];
        }

        ret_type.Resolve(code_gen);
        callisig = new PEAPI.CalliSig(call_conv,
                        ret_type.PeapiType, param_array);

        cil.calli(callisig);
    }
}
