//
// Mono.ILASM.FileRef
//
// Author(s):
//  Jackson Harper (jackson@ximian.com)
//
// Copyright 2004 Novell, Inc (http://www.novell.com)
//



namespace Mono.ILASM {

	public class FileRef {

		readonly private string name;
		readonly private byte [] hash;
		readonly private bool has_metadata;
		readonly private bool entrypoint;

		public FileRef (string name, byte[] hash, bool has_metadata, bool entrypoint)
		{
			this.name = name;
			this.hash = hash;
			this.has_metadata = has_metadata;
			this.entrypoint = entrypoint;
		}

		public void Resolve (CodeGen codegen)
		{
			codegen.PEFile.AddFile (name, hash, has_metadata, entrypoint);
		}
	}
}

