namespace MetaPrograms.CodeModel.Imperative.Members
{
    public enum MethodParameterModifier
    {
        None,
        Ref,
        RefReadonly,
        Out
        //TODO: add OutVar for out parameter with inline declaration of local variable ('declaration expression')
    }
}
