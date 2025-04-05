//
// Mono.ILASM.ExternTable.cs
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using PEAPI;
using Mobius.ILasm.interfaces;

namespace Mono.ILASM
{

    public interface IScope
    {
        ExternTypeRef GetTypeRef(string full_name, bool is_valuetype);
        ClassRef GetType(string full_name, bool is_valuetype);
        string FullName { get; }
    }

    public abstract class ExternRef : ICustomAttrTarget, IScope
    {
        protected string name;
        protected Hashtable class_table;
        protected Hashtable typeref_table;
        protected ArrayList customattr_list;
        protected bool is_resolved;
        private readonly ILogger logger;
        private readonly Dictionary<string, string> errors;

        public abstract void Resolve(CodeGen codegen);
        public abstract IExternRef GetExternRef();

        public ExternRef(string name, ILogger logger, Dictionary<string, string> errors)
        {
            this.name = name;
            this.logger = logger;
            typeref_table = [];
            class_table = [];
            this.errors = errors;
        }

        public string Name
        {
            get { return name; }
        }

        public virtual string FullName
        {
            get { return name; }
        }

        public void AddCustomAttribute(CustomAttr customattr)
        {
            customattr_list ??= [];

            customattr_list.Add(customattr);
        }

        public ExternTypeRef GetTypeRef(string full_name, bool is_valuetype)
        {
            string first = full_name;
            string rest = "";
            int slash = full_name.IndexOf('/');
            if (slash > 0)
            {
                first = full_name[..slash];
                rest = full_name[(slash + 1)..];
            }


            if (typeref_table[first] is ExternTypeRef type_ref)
            {
                if (is_valuetype && rest == "")
                    type_ref.MakeValueClass();
            }
            else
            {
                type_ref = new ExternTypeRef(this, logger, first, is_valuetype, errors);
                typeref_table[first] = type_ref;
            }

            return (rest == "" ? type_ref : type_ref.GetTypeRef(rest, is_valuetype));
        }

        public ClassRef GetType(string full_name, bool is_valuetype)
        {
            if (class_table[full_name] is ClassRef klass)
                return klass;

            ExternTable.GetNameAndNamespace(full_name, out string name_space, out string name);

            if (is_valuetype)
                klass = GetExternRef().AddValueClass(name_space, name);
            else
                klass = GetExternRef().AddClass(name_space, name);

            class_table[full_name] = klass;
            return klass;
        }

    }

    public class ExternModule : ExternRef
    {

        public ModuleRef ModuleRef;

        public ExternModule(string name, ILogger logger, Dictionary<string, string> errors) : base(name, logger, errors)
        {
        }

        public override string FullName
        {
            get
            {
                //'name' field should not contain the [.module ]
                //as its used for resolving
                return String.Format("[.module {0}]", name);
            }
        }

        public override void Resolve(CodeGen codegen)
        {
            if (is_resolved)
                return;

            ModuleRef = codegen.PEFile.AddExternModule(name);
            if (customattr_list != null)
                foreach (CustomAttr customattr in customattr_list)
                    customattr.AddTo(codegen, ModuleRef);

            is_resolved = true;
        }


        public override IExternRef GetExternRef()
        {
            return ModuleRef;
        }
    }

    public class ExternAssembly : ExternRef, IDeclSecurityTarget
    {

        public AssemblyRef AssemblyRef;

        private int major, minor, build, revision;
        private byte[] public_key;
        private byte[] public_key_token;
        private string locale;
        private byte[] hash;
        private DeclSecurity decl_sec;
        private readonly AssemblyName asmb_name;
        //flags
        private readonly AssemAttr attr;

        public ExternAssembly(string name, AssemblyName asmb_name, AssemAttr attr, ILogger logger, Dictionary<string, string> errors) : base(name, logger, errors)
        {
            this.name = name;
            this.asmb_name = asmb_name;
            this.attr = attr;
            major = minor = build = revision = -1;
        }

        public override string FullName
        {
            get
            {
                //'name' field should not contain the []
                //as its used for resolving
                return String.Format("[{0}]", name);
            }
        }

        public AssemblyName AssemblyName
        {
            get { return asmb_name; }
        }

        public DeclSecurity DeclSecurity
        {
            get
            {
                decl_sec ??= new DeclSecurity();
                return decl_sec;
            }
        }

        public override void Resolve(CodeGen code_gen)
        {
            if (is_resolved)
                return;

            AssemblyRef = code_gen.PEFile.AddExternAssembly(name);
            AssemblyRef.AddAssemblyAttr(attr);
            if (major != -1)
                AssemblyRef.AddVersionInfo(major, minor, build, revision);
            if (public_key != null)
                AssemblyRef.AddKey(public_key);
            if (public_key_token != null)
                AssemblyRef.AddKeyToken(public_key_token);
            if (locale != null)
                AssemblyRef.AddCulture(locale);
            if (hash != null)
                AssemblyRef.AddHash(hash);

            if (customattr_list != null)
                foreach (CustomAttr customattr in customattr_list)
                    customattr.AddTo(code_gen, AssemblyRef);

            decl_sec?.AddTo(code_gen, AssemblyRef);

            class_table = [];

            is_resolved = true;
        }

        public override IExternRef GetExternRef()
        {
            return AssemblyRef;
        }

        public void SetVersion(int major, int minor, int build, int revision)
        {
            this.major = major;
            this.minor = minor;
            this.build = build;
            this.revision = revision;
            asmb_name.Version = new Version(major, minor, build, revision);
        }

        public void SetPublicKey(byte[] public_key)
        {
            this.public_key = public_key;
            asmb_name.SetPublicKey(public_key);
        }

        public void SetPublicKeyToken(byte[] public_key_token)
        {
            this.public_key_token = public_key_token;
            asmb_name.SetPublicKey(public_key);
        }

        public void SetLocale(string locale)
        {
            this.locale = locale;
            //FIXME: is this correct?
            asmb_name.CultureInfo = new CultureInfo(locale);
        }

        public void SetHash(byte[] hash)
        {
            this.hash = hash;
        }

    }

    public class ExternClass
    {
        readonly string fullName;
        readonly TypeAttr ta;
        readonly string assemblyReference;

        public ExternClass(string fullName, TypeAttr ta, string assemblyReference)
        {
            this.fullName = fullName;
            this.ta = ta;
            this.assemblyReference = assemblyReference;
        }

        public void Resolve(CodeGen code_gen, ExternTable table)
        {
            var ar = table.GetAssemblyRef(assemblyReference);
            if (ar != null)
            {
                string ns = null;
                string name = fullName;

                int pos = name.LastIndexOf('.');
                if (pos > 0)
                {
                    ns = name[..pos];
                    name = name[(pos + 1)..];
                }

                code_gen.PEFile.AddExternClass(ns, name, ta, ar.AssemblyRef);
            }
        }
    }


    public class ExternTable
    {

        Hashtable assembly_table;
        Hashtable module_table;
        List<ExternClass> class_table;

        bool is_resolved;
        readonly ILogger logger;
        private readonly Dictionary<string, string> errors;

        public ExternTable(ILogger logger)
        {
            this.logger = logger;
            errors = null;
        }

        public void AddCorlib()
        {
            // Add mscorlib
            string mscorlib_name = "mscorlib";
            AssemblyName mscorlib = new AssemblyName();
            mscorlib.Name = mscorlib_name;
            AddAssembly(mscorlib_name, mscorlib, 0);

            // Also need to alias corlib, normally corlib and
            // mscorlib are used interchangably
            assembly_table["corlib"] = assembly_table["mscorlib"];
        }

        public ExternAssembly AddAssembly(string name, AssemblyName asmb_name, AssemAttr attr)
        {
            ExternAssembly ea;
            if (assembly_table == null)
            {
                assembly_table = [];
            }
            else
            {
                ea = assembly_table[name] as ExternAssembly;
                if (ea != null)
                    return ea;
            }

            ea = new ExternAssembly(name, asmb_name, attr, logger, errors);

            assembly_table[name] = ea;

            return ea;
        }

        public ExternModule AddModule(string name)
        {
            ExternModule em;
            if (module_table == null)
            {
                module_table = [];
            }
            else
            {
                em = module_table[name] as ExternModule;
                if (em != null)
                    return em;
            }

            em = new ExternModule(name, logger, errors);

            module_table[name] = em;

            return em;
        }

        public void AddClass(string name, TypeAttr ta, string assemblyReference)
        {
            class_table ??= [];

            class_table.Add(new ExternClass(name, ta, assemblyReference));
        }

        public void Resolve(CodeGen code_gen)
        {
            if (is_resolved)
                return;

            if (assembly_table != null)
                foreach (ExternAssembly ext in assembly_table.Values)
                    ext.Resolve(code_gen);

            if (module_table != null)
                foreach (ExternModule ext in module_table.Values)
                    ext.Resolve(code_gen);

            if (class_table != null)
                foreach (var entry in class_table)
                    entry.Resolve(code_gen, this);

            is_resolved = true;
        }

        public ExternTypeRef GetTypeRef(string asmb_name, string full_name, bool is_valuetype)
        {
            ExternAssembly ext_asmb = null;
            if (assembly_table == null && (asmb_name == "mscorlib" || asmb_name == "corlib"))
            {
                /* AddCorlib if mscorlib is being referenced but
                   we haven't encountered a ".assembly 'name'" as yet. */
                logger.Warning(String.Format("Reference to undeclared extern assembly '{0}', adding.", asmb_name));
                AddCorlib();
            }
            if (assembly_table != null)
                ext_asmb = assembly_table[asmb_name] as ExternAssembly;

            if (ext_asmb == null)
            {
                AssemblyName asmname = new AssemblyName();
                asmname.Name = asmb_name;

                logger.Warning(String.Format("Reference to undeclared extern assembly '{0}', adding.", asmb_name));
                ext_asmb = AddAssembly(asmb_name, asmname, 0);
            }

            return ext_asmb.GetTypeRef(full_name, is_valuetype);
        }

        public ExternTypeRef GetModuleTypeRef(string mod_name, string full_name, bool is_valuetype)
        {
            ExternModule mod = null;
            if (module_table != null)
                mod = module_table[mod_name] as ExternModule;

            if (mod == null)
            {
                logger.Error("Module " + mod_name + " not defined.");
                errors[nameof(ExternTable)] = $"Module {mod_name} not defined.";
            }

            return mod.GetTypeRef(full_name, is_valuetype);
        }

        public ExternAssembly GetAssemblyRef(string assembly_name)
        {
            ExternAssembly ass = null;
            if (assembly_table != null)
                ass = assembly_table[assembly_name] as ExternAssembly;

            if (ass == null)
            {
                logger.Error("Assembly " + assembly_name + " is not defined.");
                errors[nameof(ExternTable)] = $"Assembly {assembly_name} not defined.";
            }

            return ass;
        }

        public static void GetNameAndNamespace(string full_name,
                out string name_space, out string name)
        {

            int last_dot = full_name.LastIndexOf('.');

            if (last_dot < 0)
            {
                name_space = String.Empty;
                name = full_name;
                return;
            }

            name_space = full_name[..last_dot];
            name = full_name[(last_dot + 1)..];
        }

    }
}

