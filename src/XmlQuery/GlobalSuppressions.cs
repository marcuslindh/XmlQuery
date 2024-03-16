// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "AV1706:Identifier contains an abbreviation or is too short", Justification = "it is a char so the name c is correct.", Scope = "member", Target = "~M:XmlQuery.Query.QueryEngine.ParseQuery(System.String)~System.Collections.Generic.List{XmlQuery.Query.QueryEngineToken}")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1312:Variable names should begin with lower-case letter", Justification = "-", Scope = "member", Target = "~M:XmlQuery.Parsing.Parser.CategorizeTokens(System.Collections.Generic.List{XmlQuery.Parsing.Token})~System.Collections.Generic.List{XmlQuery.Parsing.Token}")]
[assembly: SuppressMessage("Major Code Smell", "S1066:Mergeable \"if\" statements should be combined", Justification = "-", Scope = "member", Target = "~M:XmlQuery.Parsing.Parser.CategorizeTokens(System.Collections.Generic.List{XmlQuery.Parsing.Token})~System.Collections.Generic.List{XmlQuery.Parsing.Token}")]
[assembly: SuppressMessage("Minor Code Smell", "S6602:\"Find\" method should be used instead of the \"FirstOrDefault\" extension", Justification = "-", Scope = "member", Target = "~M:XmlQuery.Xml.Element.GetAttribut(System.String,XmlQuery.Xml.Attribut@)~System.Boolean")]
[assembly: SuppressMessage("Usage", "MA0006:Use String.Equals instead of equality operator", Justification = "-", Scope = "member", Target = "~M:XmlQuery.Query.QueryEngine.Query(XmlQuery.Xml.Element,System.String)~System.Collections.Generic.List{XmlQuery.Xml.Element}")]
[assembly: SuppressMessage("Usage", "MA0002:IEqualityComparer<string> or IComparer<string> is missing", Justification = "-", Scope = "member", Target = "~P:XmlQuery.Query.ActionOnElement.Func")]
[assembly: SuppressMessage("Usage", "MA0006:Use String.Equals instead of equality operator", Justification = "-", Scope = "member", Target = "~M:XmlQuery.Parsing.Parser.CategorizeTokens(System.Collections.Generic.List{XmlQuery.Parsing.Token})~System.Collections.Generic.List{XmlQuery.Parsing.Token}")]
[assembly: SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "-", Scope = "member", Target = "~M:XmlQuery.Parsing.Parser.GroupTokens(System.Collections.Generic.List{XmlQuery.Parsing.Token})~System.Collections.Generic.List{XmlQuery.Parsing.TokenGroup}")]
[assembly: SuppressMessage("Naming", "AV1706:Identifier contains an abbreviation or is too short", Justification = "-", Scope = "member", Target = "~M:XmlQuery.Parsing.Parser.ParseTokens(System.String)~System.Collections.Generic.List{XmlQuery.Parsing.Token}")]
[assembly: SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "-", Scope = "member", Target = "~M:XmlQuery.Parsing.Parser.CategorizeTokens(System.Collections.Generic.List{XmlQuery.Parsing.Token})~System.Collections.Generic.List{XmlQuery.Parsing.Token}")]
