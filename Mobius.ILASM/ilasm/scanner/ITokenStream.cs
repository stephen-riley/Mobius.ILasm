// ITokenStream.cs
// (C) Sergey Chaban (serge@wildwestsoftware.com)


namespace Mono.ILASM {
	public interface ITokenStream {
		ILToken NextToken {get;}
		ILToken LastToken {get;}
	}
}

