//
// Mono.ILASM.CodeGen.cs
//
// Author(s):
//  Sergey Chaban (serge@wildwestsoftware.com)
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) Sergey Chaban
// (C) 2003 Jackson Harper, All rights reserved
//

using PEAPI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Security;

using SSPermissionSet = System.Security.PermissionSet;
using MIPermissionSet = Mono.ILASM.PermissionSet;

using MIAssembly = Mono.ILASM.Assembly;
using Mobius.ILasm.interfaces;

namespace Mono.ILASM
{

    public class CodeGen
    {

        private PEFile pefile;
        private ExternAssembly current_assemblyref;
        //                private ExternModule current_moduleref;
        private string current_namespace;
        private TypeDef current_typedef;
        private MethodDef current_methoddef;
        readonly private ArrayList typedef_stack;
        private int typedef_stack_top;
        readonly private SymbolWriter symwriter;
        private ICustomAttrTarget current_customattrtarget;
        private IDeclSecurityTarget current_declsectarget;
        private NativeType current_field_native_type;
        readonly private ILogger logger;

        private MIAssembly this_assembly;

        readonly private TypeManager type_manager;
        readonly private ExternTable extern_table;
        readonly private Hashtable global_field_table;
        readonly private Hashtable global_method_table;
        private Hashtable global_methodref_table;
        private Hashtable global_fieldref_table;
        readonly private Hashtable data_table;
        private FileRef file_ref;
        private ArrayList manifestResources;
        private Hashtable typeref_table;
        readonly private MemoryStream stream;
        readonly private ArrayList defcont_list;
        readonly private Dictionary<string, string> errors;

        private int sub_system;
        private int cor_flags;
        private long image_base;
        private long stack_reserve;

        readonly private string output_file;
        readonly private bool is_dll;
        private bool entry_point;
        readonly bool noautoinherit;

        private Module this_module;

        public CodeGen(ILogger logger, string output_file, MemoryStream stream, bool is_dll, bool debugging_info, bool noautoinherit, Dictionary<string, string> errors)
        {
            this.logger = logger;
            this.output_file = output_file;
            this.stream = stream;
            this.is_dll = is_dll;
            this.noautoinherit = noautoinherit;
            this.errors = errors;

            if (debugging_info)
                symwriter = new SymbolWriter(output_file);

            type_manager = new TypeManager(this);
            extern_table = new ExternTable(logger);
            typedef_stack = [];
            typedef_stack_top = 0;
            global_field_table = [];
            global_method_table = [];

            data_table = [];

            defcont_list = [];

            sub_system = -1;
            cor_flags = -1;
            image_base = -1;
            stack_reserve = -1;
            entry_point = false;
            this_module = null;
        }

        public PEFile PEFile
        {
            get { return pefile; }
        }

        public SymbolWriter SymbolWriter
        {
            get { return symwriter; }
        }

        public string CurrentNameSpace
        {
            get { return current_namespace; }
            set { current_namespace = value; }
        }

        public TypeDef CurrentTypeDef
        {
            get { return current_typedef; }
        }

        public MethodDef CurrentMethodDef
        {
            get { return current_methoddef; }
        }

        public ExternAssembly CurrentAssemblyRef
        {
            get { return current_assemblyref; }
        }

        //                public ExternModule CurrentModuleRef {
        //                        get { return current_moduleref; }
        //                }

        public ICustomAttrTarget CurrentCustomAttrTarget
        {
            get { return current_customattrtarget; }
            set { current_customattrtarget = value; }
        }

        public IDeclSecurityTarget CurrentDeclSecurityTarget
        {
            get { return current_declsectarget; }
            set { current_declsectarget = value; }
        }

        public ExternTable ExternTable
        {
            get { return extern_table; }
        }

        public TypeManager TypeManager
        {
            get { return type_manager; }
        }

        public bool HasEntryPoint
        {
            get { return entry_point; }
            set
            {
                /* if (!value) error: unsetting entrypoint ? */
                if (entry_point)
                {
                    logger.Error("Multiple .entrypoint declarations.");
                    errors[nameof(CodeGen)] = "Multiple .entrypoint declarations.";
                }
                entry_point = value;
            }
        }

        public TypeRef GetTypeRef(string name)
        {
            TypeRef tr = null;

            if (typeref_table == null)
                typeref_table = [];
            else
                tr = typeref_table[name] as TypeRef;

            if (tr == null)
            {
                tr = new TypeRef(logger, name, false, null, errors);
                typeref_table[name] = tr;
            }

            return tr;
        }

        public GlobalMethodRef GetGlobalMethodRef(BaseTypeRef ret_type, CallConv call_conv,
                        string name, BaseTypeRef[] param, int gen_param_count)
        {
            string key = MethodDef.CreateSignature(ret_type, call_conv, name, param, gen_param_count, true);

            GlobalMethodRef methref = null;

            if (global_methodref_table == null)
                global_methodref_table = [];
            else
                methref = (GlobalMethodRef)global_methodref_table[key];

            if (methref == null)
            {
                methref = new GlobalMethodRef(ret_type, call_conv, name, param, gen_param_count, logger, errors);
                global_methodref_table[key] = methref;
            }

            return methref;
        }

        public GlobalFieldRef GetGlobalFieldRef(BaseTypeRef ret_type, string name)
        {
            string key = ret_type.FullName + name;

            GlobalFieldRef fieldref = null;

            if (global_fieldref_table == null)
                global_fieldref_table = [];
            else
                fieldref = (GlobalFieldRef)global_fieldref_table[key];

            if (fieldref == null)
            {
                fieldref = new GlobalFieldRef(ret_type, name);
                global_fieldref_table[key] = fieldref;
            }

            return fieldref;
        }

        public void SetSubSystem(int sub_system)
        {
            this.sub_system = sub_system;
        }

        public void SetCorFlags(int cor_flags)
        {
            this.cor_flags = cor_flags;
        }

        public void SetImageBase(long image_base)
        {
            this.image_base = image_base;
        }

        public void SetStackReserve(long stack_reserve)
        {
            this.stack_reserve = stack_reserve;
        }

        public void SetThisAssembly(string name, AssemAttr attr)
        {
            if (this_assembly != null && this_assembly.Name != name)
            {
                logger.Error("Multiple assembly declarations");
                errors[nameof(CodeGen)] = "Multiple assembly declarations";
            }
            this_assembly = new MIAssembly(name);
            this_assembly.SetAssemblyAttr(attr);
            if (name != "mscorlib")
                ExternTable.AddCorlib();
        }

        public void SetModuleName(string module_name)
        {
            this_module = new Module(module_name, logger, errors);
            CurrentCustomAttrTarget = this_module;
        }

        public void SetFileRef(FileRef file_ref)
        {
            this.file_ref = file_ref;
        }

        public MIAssembly ThisAssembly
        {
            get { return this_assembly; }
        }

        public bool IsThisAssembly(string name)
        {
            return (this_assembly != null && name == this_assembly.Name);
        }

        public Module ThisModule
        {
            get { return this_module; }
        }

        public bool IsThisModule(string name)
        {
            return (this_module != null && name == this_module.Name);
        }

        public void BeginSourceFile(string name)
        {
            symwriter?.BeginSourceFile(name);
        }

        public void EndSourceFile()
        {
            symwriter?.EndSourceFile();
        }

        public void BeginTypeDef(TypeAttr attr, string name, BaseClassRef parent,
                        ArrayList impl_list, Location location, GenericParameters gen_params)
        {
            TypeDef outer = null;
            string cache_name = CacheName(name);
            if (typedef_stack_top > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < typedef_stack_top; i++)
                {
                    outer = (TypeDef)typedef_stack[i];
                    if (i == 0)
                        /* Use FullName for outermost class to get the
						   namespace also */
                        sb.Append(outer.FullName);
                    else
                        sb.Append(outer.Name);
                    sb.Append('/');
                }
                sb.Append(name);
                cache_name = sb.ToString();
            }

            TypeDef typedef = type_manager[cache_name];

            if (typedef != null)
            {
                // Class head is allready defined, we are just reopening the class
                current_customattrtarget = current_typedef = typedef;
                current_declsectarget = typedef;
                typedef_stack.Add(current_typedef);
                typedef_stack_top++;
                return;
            }

            typedef = new TypeDef(attr, current_namespace,
                            name, parent, impl_list, location, gen_params, outer, logger, errors);
            typedef.NoAutoInherit = noautoinherit && parent == null;
            type_manager[cache_name] = typedef;
            current_customattrtarget = current_typedef = typedef;
            current_declsectarget = typedef;
            typedef_stack.Add(typedef);
            typedef_stack_top++;
        }

        public void AddFieldMarshalInfo(NativeType native_type)
        {
            current_field_native_type = native_type;
        }

        public void AddFieldDef(FieldDef fielddef)
        {
            if (current_field_native_type != null)
            {
                fielddef.AddMarshalInfo(current_field_native_type);
                current_field_native_type = null;
            }

            if (current_typedef != null)
            {
                current_typedef.AddFieldDef(fielddef);
            }
            else
            {
                global_field_table.Add(
                        new DictionaryEntry(fielddef.Name, fielddef.Type.FullName),
                        fielddef);
            }
        }

        public void AddDataDef(DataDef datadef)
        {
            if (data_table[datadef.Name] != null)
            {
                logger.Error("Duplicate global label '" + datadef.Name + "'");
                errors[nameof(CodeGen)] = $"Duplicate global label '{datadef.Name}'";
            }
            data_table[datadef.Name] = datadef;
        }

        public void AddManifestResource(ManifestResource mr)
        {
            manifestResources ??= [];
            manifestResources.Add(mr);
        }

        public DataConstant GetDataConst(string name)
        {
            DataDef def = (DataDef)data_table[name];
            if (def == null)
                return null;

            return (DataConstant)def.PeapiConstant;
        }

        public void BeginMethodDef(MethodDef methoddef)
        {
            if (current_typedef != null)
            {
                current_typedef.AddMethodDef(methoddef);
            }
            else
            {
                global_method_table.Add(methoddef.Signature,
                                methoddef);
            }

            current_customattrtarget = current_methoddef = methoddef;
            current_declsectarget = methoddef;
        }

        public void EndMethodDef(Location location)
        {
            symwriter?.EndMethod(location);

            current_methoddef = null;
        }

        public void EndTypeDef()
        {
            typedef_stack_top--;
            typedef_stack.RemoveAt(typedef_stack_top);

            if (typedef_stack_top > 0)
                current_typedef = (TypeDef)typedef_stack[typedef_stack_top - 1];
            else
                current_typedef = null;

        }

        public void BeginAssemblyRef(string name, AssemblyName asmb_name, AssemAttr attr)
        {
            current_customattrtarget = current_assemblyref = ExternTable.AddAssembly(name, asmb_name, attr);
            current_declsectarget = current_assemblyref;
        }

        public void EndAssemblyRef()
        {
            current_assemblyref = null;
            current_customattrtarget = null;
            current_declsectarget = null;
        }

        public void AddToDefineContentsList(TypeDef typedef)
        {
            defcont_list.Add(typedef);
        }

        public void AddPermission(SecurityAction sec_action, object perm)
        {
            if (CurrentDeclSecurityTarget == null)
                return;

            AddPermission(sec_action, perm, CurrentDeclSecurityTarget.DeclSecurity);
        }

        private static void AddPermission(SecurityAction sec_action, object perm, DeclSecurity decl_sec)
        {
            if (perm is SSPermissionSet ps)
            {
                decl_sec.AddPermissionSet(sec_action, ps);
                return;
            }

            if (perm is IPermission iper)
            {
                decl_sec.AddPermission(sec_action, iper);
                return;
            }

            if (perm is MIPermissionSet ps20)
            {
                decl_sec.AddPermissionSet(sec_action, ps20);
                return;
            }
        }

        public void Write()
        {
            //FileStream out_stream = null;

            try
            {
                if (ThisModule == null)
                    this_module = new Module(Path.GetFileName(output_file), logger, errors);

                //out_stream = new FileStream(output_file, FileMode.Create, FileAccess.Write);

                pefile = new PEFile(ThisAssembly?.Name, ThisModule.Name, is_dll, ThisAssembly != null, null, stream);
                PEAPI.Assembly asmb = pefile.GetThisAssembly();

                ThisModule.PeapiModule = pefile.GetThisModule();

                file_ref?.Resolve(this);

                extern_table.Resolve(this);
                type_manager.DefineAll();

                if (manifestResources != null)
                {
                    foreach (ManifestResource mr in manifestResources)
                        pefile.AddManifestResource(mr);
                }

                foreach (FieldDef fielddef in global_field_table.Values)
                {
                    fielddef.Define(this);
                }

                foreach (MethodDef methoddef in global_method_table.Values)
                {
                    methoddef.Define(this);
                }

                foreach (TypeDef typedef in defcont_list)
                {
                    typedef.DefineContents(this);
                }

                ThisAssembly?.Resolve(this, pefile.GetThisAssembly());
                ThisModule.Resolve(this, pefile.GetThisModule());

                if (sub_system != -1)
                    pefile.SetSubSystem((SubSystem)sub_system);
                if (cor_flags != -1)
                    pefile.SetCorFlags(cor_flags);
                if (stack_reserve != -1)
                    pefile.SetStackReserve(stack_reserve);

                //PEAPI closes the stream as expected as it was using a filestream
                //but given that we are now using a memory stream, the stream close
                //has been commented out. Need to revisit the code to 
                pefile.WritePEFile();

                if (symwriter != null)
                {
                    Guid guid = pefile.GetThisModule().Guid;
                    symwriter.Write(guid);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            //finally
            //{
            //    if (stream != null)
            //        stream.Close();
            //}
        }

        public Method ResolveMethod(string signature)
        {
            MethodDef methoddef = (MethodDef)global_method_table[signature];
            if (methoddef == null)
            {
                logger.Error("Unable to resolve global method : " + signature);
                errors[nameof(CodeGen)] = $"Unable to resolve global method : {signature}";
            }

            return methoddef.Resolve(this);
        }

        public Method ResolveVarargMethod(string sig_only_required_params, string sig_with_optional_params,
                        CodeGen code_gen, PEAPI.Type[] opt)
        {
            MethodDef methoddef = (MethodDef)global_method_table[sig_only_required_params];
            if (methoddef == null)
            {
                logger.Error("Unable to resolve global method : " + sig_only_required_params);
                errors[nameof(CodeGen)] = $"Unable to resolve global method : {sig_only_required_params}";
            }

            methoddef.Resolve(code_gen);
            return methoddef.GetVarargSig(opt, sig_with_optional_params);
        }

        public Field ResolveField(string name, string type_name)
        {
            FieldDef fielddef = (FieldDef)global_field_table[new DictionaryEntry(name, type_name)];
            if (fielddef == null)
            {
                logger.Error(String.Format("Unable to resolve global field : {0} {1}", type_name, name));
                errors[nameof(CodeGen)] = $"Unable to resolve global field : {type_name} {name}";
            }
            return fielddef.Resolve(this);
        }

        private string CacheName(string name)
        {
            if (current_namespace == null ||
                            current_namespace == String.Empty)
                return name;

            return current_namespace + "." + name;
        }
    }

}

