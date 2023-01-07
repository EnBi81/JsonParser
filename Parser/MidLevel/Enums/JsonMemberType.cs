namespace Parser.Analyzer.Enums;

public enum JsonMemberType
{
    /// <summary>
    /// The member is static
    /// </summary>
    Static, 
    /// <summary>
    /// The json member is an iterator
    /// </summary>
    Iterator, 
    /// <summary>
    /// The json member is inside an iterator
    /// </summary>
    Instance, 
    /// <summary>
    /// The json member is a sub iterator (iterator inside an iterator
    /// </summary>
    MultiValuedIterator,
    /// <summary>
    /// The json member is inside an MultiValuedIterator
    /// </summary>
    MultiValuedInstance
}