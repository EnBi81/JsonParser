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
    /// The json member is in an iterator and is multivalued (list of a list)
    /// </summary>
    MultiValuedInstance
}