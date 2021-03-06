﻿using Capgemini.CodeAnalysis.CoreAnalysers.Extensions;
using Capgemini.CodeAnalysis.CoreAnalysers.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Capgemini.CodeAnalysis.CoreAnalysers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PrivateFieldNamingUnderscoreAnalyzer : AnalyzerBase
    {
        static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(AnalyzerType.PrivateFieldNamingUnderscoreAnalyzerId.ToDiagnosticId(),
            nameof(PrivateFieldNamingUnderscoreAnalyzer), $"{nameof(PrivateFieldNamingUnderscoreAnalyzer)}: Field '{{0}}' does not start with '_'", "Naming", DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        }

        private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGeneratedCode())
            {
                return;
            }

            var declaration = Cast<FieldDeclarationSyntax>(context.Node);

            var variableName = declaration.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
            var location = declaration.Declaration.Variables.FirstOrDefault()?.Identifier.GetLocation();

            if (!string.IsNullOrWhiteSpace(variableName) && !IsExternallyVisible(declaration.Modifiers))
            {
                if (!variableName.StartsWith("_", StringComparison.Ordinal))
                {
                    var diagnostic = Diagnostic.Create(Rule, location, variableName);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}