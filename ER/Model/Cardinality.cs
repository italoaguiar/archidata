using System;

[Serializable]
public enum Cardinality
{
    /// <summary>
    /// Representa uma cardinalidade multipla.
    /// </summary>
    /// <example>
    /// 1 -> M = ONE -> MULTI
    /// </example>
    MULTI,
    /// <summary>
    /// Representa uma cardinalidade individual.
    /// </summary>
    /// <example>
    /// 1 -> M = ONE -> MULTI
    /// </example>
    ONE
}
