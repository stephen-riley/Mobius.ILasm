//
// Mono.ILASM.InstrTable
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//

using PEAPI;
using System.Collections;

namespace Mono.ILASM
{

    public class InstrTable
    {

        private static Hashtable inst_table;

        static InstrTable()
        {
            CreateInstTable();
        }

        public static ILToken GetToken(string str)
        {
            return inst_table[str] as ILToken;
        }

        public static bool IsInstr(string str)
        {
            return inst_table.Contains(str);
        }

        private static void CreateInstTable()
        {
            inst_table = new Hashtable
            {
                ["nop"] = new ILToken(Token.INSTR_NONE, Op.nop),
                ["break"] = new ILToken(Token.INSTR_NONE, Op.breakOp),
                ["ldarg.0"] = new ILToken(Token.INSTR_NONE, Op.ldarg_0),
                ["ldarg.1"] = new ILToken(Token.INSTR_NONE, Op.ldarg_1),
                ["ldarg.2"] = new ILToken(Token.INSTR_NONE, Op.ldarg_2),
                ["ldarg.3"] = new ILToken(Token.INSTR_NONE, Op.ldarg_3),
                ["ldloc.0"] = new ILToken(Token.INSTR_NONE, Op.ldloc_0),
                ["ldloc.1"] = new ILToken(Token.INSTR_NONE, Op.ldloc_1),
                ["ldloc.2"] = new ILToken(Token.INSTR_NONE, Op.ldloc_2),
                ["ldloc.3"] = new ILToken(Token.INSTR_NONE, Op.ldloc_3),
                ["stloc.0"] = new ILToken(Token.INSTR_NONE, Op.stloc_0),
                ["stloc.1"] = new ILToken(Token.INSTR_NONE, Op.stloc_1),
                ["stloc.2"] = new ILToken(Token.INSTR_NONE, Op.stloc_2),
                ["stloc.3"] = new ILToken(Token.INSTR_NONE, Op.stloc_3),
                ["ldnull"] = new ILToken(Token.INSTR_NONE, Op.ldnull),
                ["ldc.i4.m1"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_m1),
                ["ldc.i4.M1"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_m1),
                ["ldc.i4.0"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_0),
                ["ldc.i4.1"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_1),
                ["ldc.i4.2"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_2),
                ["ldc.i4.3"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_3),
                ["ldc.i4.4"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_4),
                ["ldc.i4.5"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_5),
                ["ldc.i4.6"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_6),
                ["ldc.i4.7"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_7),
                ["ldc.i4.8"] = new ILToken(Token.INSTR_NONE, Op.ldc_i4_8),
                ["dup"] = new ILToken(Token.INSTR_NONE, Op.dup),
                ["pop"] = new ILToken(Token.INSTR_NONE, Op.pop),
                ["ret"] = new ILToken(Token.INSTR_NONE, Op.ret),
                ["ldind.i1"] = new ILToken(Token.INSTR_NONE, Op.ldind_i1),
                ["ldind.u1"] = new ILToken(Token.INSTR_NONE, Op.ldind_u1),
                ["ldind.i2"] = new ILToken(Token.INSTR_NONE, Op.ldind_i2),
                ["ldind.u2"] = new ILToken(Token.INSTR_NONE, Op.ldind_u2),
                ["ldind.i4"] = new ILToken(Token.INSTR_NONE, Op.ldind_i4),
                ["ldind.u4"] = new ILToken(Token.INSTR_NONE, Op.ldind_u4),
                ["ldind.i8"] = new ILToken(Token.INSTR_NONE, Op.ldind_i8),
                ["ldind.u8"] = new ILToken(Token.INSTR_NONE, Op.ldind_i8),
                ["ldind.i"] = new ILToken(Token.INSTR_NONE, Op.ldind_i),
                ["ldind.r4"] = new ILToken(Token.INSTR_NONE, Op.ldind_r4),
                ["ldind.r8"] = new ILToken(Token.INSTR_NONE, Op.ldind_r8),
                ["ldind.ref"] = new ILToken(Token.INSTR_NONE, Op.ldind_ref),
                ["stind.ref"] = new ILToken(Token.INSTR_NONE, Op.stind_ref),
                ["stind.i1"] = new ILToken(Token.INSTR_NONE, Op.stind_i1),
                ["stind.i2"] = new ILToken(Token.INSTR_NONE, Op.stind_i2),
                ["stind.i4"] = new ILToken(Token.INSTR_NONE, Op.stind_i4),
                ["stind.i8"] = new ILToken(Token.INSTR_NONE, Op.stind_i8),
                ["stind.r4"] = new ILToken(Token.INSTR_NONE, Op.stind_r4),
                ["stind.r8"] = new ILToken(Token.INSTR_NONE, Op.stind_r8),
                ["add"] = new ILToken(Token.INSTR_NONE, Op.add),
                ["sub"] = new ILToken(Token.INSTR_NONE, Op.sub),
                ["mul"] = new ILToken(Token.INSTR_NONE, Op.mul),
                ["div"] = new ILToken(Token.INSTR_NONE, Op.div),
                ["div.un"] = new ILToken(Token.INSTR_NONE, Op.div_un),
                ["rem"] = new ILToken(Token.INSTR_NONE, Op.rem),
                ["rem.un"] = new ILToken(Token.INSTR_NONE, Op.rem_un),
                ["and"] = new ILToken(Token.INSTR_NONE, Op.and),
                ["or"] = new ILToken(Token.INSTR_NONE, Op.or),
                ["xor"] = new ILToken(Token.INSTR_NONE, Op.xor),
                ["shl"] = new ILToken(Token.INSTR_NONE, Op.shl),
                ["shr"] = new ILToken(Token.INSTR_NONE, Op.shr),
                ["shr.un"] = new ILToken(Token.INSTR_NONE, Op.shr_un),
                ["neg"] = new ILToken(Token.INSTR_NONE, Op.neg),
                ["not"] = new ILToken(Token.INSTR_NONE, Op.not),
                ["conv.i1"] = new ILToken(Token.INSTR_NONE, Op.conv_i1),
                ["conv.i2"] = new ILToken(Token.INSTR_NONE, Op.conv_i2),
                ["conv.i4"] = new ILToken(Token.INSTR_NONE, Op.conv_i4),
                ["conv.i8"] = new ILToken(Token.INSTR_NONE, Op.conv_i8),
                ["conv.r4"] = new ILToken(Token.INSTR_NONE, Op.conv_r4),
                ["conv.r8"] = new ILToken(Token.INSTR_NONE, Op.conv_r8),
                ["conv.u4"] = new ILToken(Token.INSTR_NONE, Op.conv_u4),
                ["conv.u8"] = new ILToken(Token.INSTR_NONE, Op.conv_u8),
                ["conv.r.un"] = new ILToken(Token.INSTR_NONE, Op.conv_r_un),
                ["throw"] = new ILToken(Token.INSTR_NONE, Op.throwOp),
                ["conv.ovf.i1.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i1_un),
                ["conv.ovf.i2.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i2_un),
                ["conv.ovf.i4.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i4_un),
                ["conv.ovf.i8.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i8_un),
                ["conf.ovf.u1.un"] = new ILToken(Token.INSTR_NONE, Op.conf_ovf_u1_un),
                ["conv.ovf.u2.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u2_un),
                ["conv.ovf.u4.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u4_un),
                ["conv.ovf.u8.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u8_un),
                ["conv.ovf.i.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i_un),
                ["conv.ovf.u.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u_un),
                ["ldlen"] = new ILToken(Token.INSTR_NONE, Op.ldlen),
                ["ldelem.i1"] = new ILToken(Token.INSTR_NONE, Op.ldelem_i1),
                ["ldelem.u1"] = new ILToken(Token.INSTR_NONE, Op.ldelem_u1),
                ["ldelem.i2"] = new ILToken(Token.INSTR_NONE, Op.ldelem_i2),
                ["ldelem.u2"] = new ILToken(Token.INSTR_NONE, Op.ldelem_u2),
                ["ldelem.i4"] = new ILToken(Token.INSTR_NONE, Op.ldelem_i4),
                ["ldelem.u4"] = new ILToken(Token.INSTR_NONE, Op.ldelem_u4),
                ["ldelem.i8"] = new ILToken(Token.INSTR_NONE, Op.ldelem_i8),
                ["ldelem.u8"] = new ILToken(Token.INSTR_NONE, Op.ldelem_i8),
                ["ldelem.i"] = new ILToken(Token.INSTR_NONE, Op.ldelem_i),
                ["ldelem.r4"] = new ILToken(Token.INSTR_NONE, Op.ldelem_r4),
                ["ldelem.r8"] = new ILToken(Token.INSTR_NONE, Op.ldelem_r8),
                ["ldelem.ref"] = new ILToken(Token.INSTR_NONE, Op.ldelem_ref),
                ["stelem.i"] = new ILToken(Token.INSTR_NONE, Op.stelem_i),
                ["stelem.i1"] = new ILToken(Token.INSTR_NONE, Op.stelem_i1),
                ["stelem.i2"] = new ILToken(Token.INSTR_NONE, Op.stelem_i2),
                ["stelem.i4"] = new ILToken(Token.INSTR_NONE, Op.stelem_i4),
                ["stelem.i8"] = new ILToken(Token.INSTR_NONE, Op.stelem_i8),
                ["stelem.r4"] = new ILToken(Token.INSTR_NONE, Op.stelem_r4),
                ["stelem.r8"] = new ILToken(Token.INSTR_NONE, Op.stelem_r8),
                ["stelem.ref"] = new ILToken(Token.INSTR_NONE, Op.stelem_ref),
                ["conv.ovf.i1"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i1),
                ["conv.ovf.u1"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u1),
                ["conv.ovf.i2"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i2),
                ["conv.ovf.u2"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u2),
                ["conv.ovf.i4"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i4),
                ["conv.ovf.u4"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u4),
                ["conv.ovf.i8"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i8),
                ["conv.ovf.u8"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u8),
                ["conv.ovf.u1.un"] = new ILToken(Token.INSTR_NONE, Op.conf_ovf_u1_un),
                ["conv.ovf.u2.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u2_un),
                ["conv.ovf.u4.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u4_un),
                ["conv.ovf.u8.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u8_un),
                ["conv.ovf.i1.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i1_un),
                ["conv.ovf.i2.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i2_un),
                ["conv.ovf.i4.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i4_un),
                ["conv.ovf.i8.un"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i8_un),
                ["ckfinite"] = new ILToken(Token.INSTR_NONE, Op.ckfinite),
                ["conv.u2"] = new ILToken(Token.INSTR_NONE, Op.conv_u2),
                ["conv.u1"] = new ILToken(Token.INSTR_NONE, Op.conv_u1),
                ["conv.i"] = new ILToken(Token.INSTR_NONE, Op.conv_i),
                ["conv.ovf.i"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_i),
                ["conv.ovf.u"] = new ILToken(Token.INSTR_NONE, Op.conv_ovf_u),
                ["add.ovf"] = new ILToken(Token.INSTR_NONE, Op.add_ovf),
                ["add.ovf.un"] = new ILToken(Token.INSTR_NONE, Op.add_ovf_un),
                ["mul.ovf"] = new ILToken(Token.INSTR_NONE, Op.mul_ovf),
                ["mul.ovf.un"] = new ILToken(Token.INSTR_NONE, Op.mul_ovf_un),
                ["sub.ovf"] = new ILToken(Token.INSTR_NONE, Op.sub_ovf),
                ["sub.ovf.un"] = new ILToken(Token.INSTR_NONE, Op.sub_ovf_un),
                ["endfinally"] = new ILToken(Token.INSTR_NONE, Op.endfinally),
                // endfault is really just an alias for endfinally
                ["endfault"] = new ILToken(Token.INSTR_NONE, Op.endfinally),
                ["stind.i"] = new ILToken(Token.INSTR_NONE, Op.stind_i),
                ["conv.u"] = new ILToken(Token.INSTR_NONE, Op.conv_u),
                ["arglist"] = new ILToken(Token.INSTR_NONE, Op.arglist),
                ["ceq"] = new ILToken(Token.INSTR_NONE, Op.ceq),
                ["cgt"] = new ILToken(Token.INSTR_NONE, Op.cgt),
                ["cgt.un"] = new ILToken(Token.INSTR_NONE, Op.cgt_un),
                ["clt"] = new ILToken(Token.INSTR_NONE, Op.clt),
                ["clt.un"] = new ILToken(Token.INSTR_NONE, Op.clt_un),
                ["localloc"] = new ILToken(Token.INSTR_NONE, Op.localloc),
                ["endfilter"] = new ILToken(Token.INSTR_NONE, Op.endfilter),
                ["volatile."] = new ILToken(Token.INSTR_NONE, Op.volatile_),
                ["tail."] = new ILToken(Token.INSTR_NONE, Op.tail_),
                ["cpblk"] = new ILToken(Token.INSTR_NONE, Op.cpblk),
                ["initblk"] = new ILToken(Token.INSTR_NONE, Op.initblk),
                ["rethrow"] = new ILToken(Token.INSTR_NONE, Op.rethrow),
                ["refanytype"] = new ILToken(Token.INSTR_NONE, Op.refanytype),
                ["readonly."] = new ILToken(Token.INSTR_NONE, Op.readonly_),

                //
                // Int operations
                //

                // param
                ["ldarg"] = new ILToken(Token.INSTR_PARAM, IntOp.ldarg),
                ["ldarga"] = new ILToken(Token.INSTR_PARAM, IntOp.ldarga),
                ["starg"] = new ILToken(Token.INSTR_PARAM, IntOp.starg),
                ["ldarg.s"] = new ILToken(Token.INSTR_PARAM, IntOp.ldarg_s),
                ["ldarga.s"] = new ILToken(Token.INSTR_PARAM, IntOp.ldarga_s),
                ["starg.s"] = new ILToken(Token.INSTR_PARAM, IntOp.starg_s),

                // local
                ["ldloc"] = new ILToken(Token.INSTR_LOCAL, IntOp.ldloc),
                ["ldloca"] = new ILToken(Token.INSTR_LOCAL, IntOp.ldloca),
                ["stloc"] = new ILToken(Token.INSTR_LOCAL, IntOp.stloc),
                ["ldloc.s"] = new ILToken(Token.INSTR_LOCAL, IntOp.ldloc_s),
                ["ldloca.s"] = new ILToken(Token.INSTR_LOCAL, IntOp.ldloca_s),
                ["stloc.s"] = new ILToken(Token.INSTR_LOCAL, IntOp.stloc_s),

                ["ldc.i4.s"] = new ILToken(Token.INSTR_I, IntOp.ldc_i4_s),
                ["ldc.i4"] = new ILToken(Token.INSTR_I, IntOp.ldc_i4),
                ["unaligned."] = new ILToken(Token.INSTR_I, IntOp.unaligned),

                //
                // Type operations
                //

                ["cpobj"] = new ILToken(Token.INSTR_TYPE, TypeOp.cpobj),
                ["ldobj"] = new ILToken(Token.INSTR_TYPE, TypeOp.ldobj),
                ["castclass"] = new ILToken(Token.INSTR_TYPE, TypeOp.castclass),
                ["isinst"] = new ILToken(Token.INSTR_TYPE, TypeOp.isinst),
                ["unbox"] = new ILToken(Token.INSTR_TYPE, TypeOp.unbox),
                ["unbox.any"] = new ILToken(Token.INSTR_TYPE, TypeOp.unbox_any),
                ["stobj"] = new ILToken(Token.INSTR_TYPE, TypeOp.stobj),
                ["box"] = new ILToken(Token.INSTR_TYPE, TypeOp.box),
                ["newarr"] = new ILToken(Token.INSTR_TYPE, TypeOp.newarr),
                ["ldelema"] = new ILToken(Token.INSTR_TYPE, TypeOp.ldelema),
                ["refanyval"] = new ILToken(Token.INSTR_TYPE, TypeOp.refanyval),
                ["mkrefany"] = new ILToken(Token.INSTR_TYPE, TypeOp.mkrefany),
                ["initobj"] = new ILToken(Token.INSTR_TYPE, TypeOp.initobj),
                ["sizeof"] = new ILToken(Token.INSTR_TYPE, TypeOp.sizeOf),
                ["stelem"] = new ILToken(Token.INSTR_TYPE, TypeOp.stelem),
                ["ldelem"] = new ILToken(Token.INSTR_TYPE, TypeOp.ldelem),
                ["stelem.any"] = new ILToken(Token.INSTR_TYPE, TypeOp.stelem),
                ["ldelem.any"] = new ILToken(Token.INSTR_TYPE, TypeOp.ldelem),
                ["constrained."] = new ILToken(Token.INSTR_TYPE, TypeOp.constrained),

                //
                // MethodRef operations
                //

                ["jmp"] = new ILToken(Token.INSTR_METHOD, MethodOp.jmp),
                ["call"] = new ILToken(Token.INSTR_METHOD, MethodOp.call),
                ["callvirt"] = new ILToken(Token.INSTR_METHOD, MethodOp.callvirt),
                ["newobj"] = new ILToken(Token.INSTR_METHOD, MethodOp.newobj),
                ["ldftn"] = new ILToken(Token.INSTR_METHOD, MethodOp.ldftn),
                ["ldvirtftn"] = new ILToken(Token.INSTR_METHOD, MethodOp.ldvirtfn),

                //
                // FieldRef instructions
                //

                ["ldfld"] = new ILToken(Token.INSTR_FIELD, FieldOp.ldfld),
                ["ldflda"] = new ILToken(Token.INSTR_FIELD, FieldOp.ldflda),
                ["stfld"] = new ILToken(Token.INSTR_FIELD, FieldOp.stfld),
                ["ldsfld"] = new ILToken(Token.INSTR_FIELD, FieldOp.ldsfld),
                ["ldsflda"] = new ILToken(Token.INSTR_FIELD, FieldOp.ldsflda),
                ["stsfld"] = new ILToken(Token.INSTR_FIELD, FieldOp.stsfld),

                //
                // Branch Instructions
                //

                ["br"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.br),
                ["brfalse"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.brfalse),
                ["brzero"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.brfalse),
                ["brnull"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.brfalse),
                ["brtrue"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.brtrue),
                ["beq"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.beq),
                ["bge"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bge),
                ["bgt"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bgt),
                ["ble"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.ble),
                ["blt"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.blt),
                ["bne.un"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bne_un),
                ["bge.un"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bge_un),
                ["bgt.un"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bgt_un),
                ["ble.un"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.ble_un),
                ["blt.un"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.blt_un),
                ["leave"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.leave),

                ["br.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.br_s),
                ["brfalse.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.brfalse_s),
                ["brtrue.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.brtrue_s),
                ["beq.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.beq_s),
                ["bge.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bge_s),
                ["bgt.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bgt_s),
                ["ble.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.ble_s),
                ["blt.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.blt_s),
                ["bne.un.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bne_un_s),
                ["bge.un.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bge_un_s),
                ["bgt.un.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.bgt_un_s),
                ["ble.un.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.ble_un_s),
                ["blt.un.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.blt_un_s),
                ["leave.s"] = new ILToken(Token.INSTR_BRTARGET, BranchOp.leave_s),

                //
                // Misc other instructions
                //

                ["ldstr"] = new ILToken(Token.INSTR_STRING, MiscInstr.ldstr),
                ["ldc.r4"] = new ILToken(Token.INSTR_R, MiscInstr.ldc_r4),
                ["ldc.r8"] = new ILToken(Token.INSTR_R, MiscInstr.ldc_r8),
                ["ldc.i8"] = new ILToken(Token.INSTR_I8, MiscInstr.ldc_i8),
                ["switch"] = new ILToken(Token.INSTR_SWITCH, MiscInstr._switch),
                ["calli"] = new ILToken(Token.INSTR_SIG, MiscInstr.calli),
                ["ldtoken"] = new ILToken(Token.INSTR_TOK, MiscInstr.ldtoken)
            };
        }

    }

}

