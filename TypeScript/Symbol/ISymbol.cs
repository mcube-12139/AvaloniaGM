using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Type;

namespace TypeScript.Symbol;

public interface ISymbol {
    string GetName();
    internal IValueSymbol AsValue(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NOT_VALUE, [GetName()], position);
    }
    internal ITypeSymbol AsType(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NOT_TYPE, [GetName()], position);
    }
    internal IConceptSymbol AsConcept(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NOT_CONCEPT, [GetName()], position);
    }
    internal IGenericSymbol AsGeneric(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NOT_GENERIC, [GetName()], position);
    }
    internal IValueSymbol GetGenericOmittedCallSymbol(IType? target, IExpression[] parameters, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_CALLABLE, [GetName()], position);
    }
    internal NameSpace GetNameSpace(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NO_NAME_SPACE, [GetName()], position);
    }
    internal Dictionary<string, ConceptFunctionSymbol> GetConceptFunctions(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NOT_CONCEPT, [GetName()], position);
    }
    internal IType GetMethodCallResultType(IExpression[] parameterTypes, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.METHOD_NOT_EXIST, [GetName()], position);
    }
}
