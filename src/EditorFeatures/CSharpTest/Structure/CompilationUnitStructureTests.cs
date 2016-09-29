﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Structure;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Structure;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Structure
{
    public class CompilationUnitStructureTests : AbstractCSharpSyntaxNodeStructureTests<CompilationUnitSyntax>
    {
        internal override AbstractSyntaxStructureProvider CreateProvider() => new CompilationUnitStructureProvider();

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestUsings()
        {
            const string code = @"
$${|hint:using {|collapse:System;
using System.Core;|}|}";

            await VerifyBlockSpansAsync(code,
                Region("collapse", "hint", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestUsingAliases()
        {
            const string code = @"
$${|hint:using {|collapse:System;
using System.Core;
using text = System.Text;
using linq = System.Linq;|}|}";

            await VerifyBlockSpansAsync(code,
                Region("collapse", "hint", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestExternAliases()
        {
            const string code = @"
$${|hint:extern {|collapse:alias Foo;
extern alias Bar;|}|}";

            await VerifyBlockSpansAsync(code,
                Region("collapse", "hint", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestExternAliasesAndUsings()
        {
            const string code = @"
$${|hint:extern {|collapse:alias Foo;
extern alias Bar;
using System;
using System.Core;|}|}";

            await VerifyBlockSpansAsync(code,
                Region("collapse", "hint", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestExternAliasesAndUsingsWithLeadingTrailingAndNestedComments()
        {
            const string code = @"
$${|span1:// Foo
// Bar|}
{|hint2:extern {|collapse2:alias Foo;
extern alias Bar;
// Foo
// Bar
using System;
using System.Core;|}|}
{|span3:// Foo
// Bar|}";

            await VerifyBlockSpansAsync(code,
                Region("span1", "// Foo ...", autoCollapse: true),
                Region("collapse2", "hint2", CSharpStructureHelpers.Ellipsis, autoCollapse: true),
                Region("span3", "// Foo ...", autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestUsingsWithComments()
        {
            const string code = @"
$${|span1:// Foo
// Bar|}
{|hint2:using {|collapse2:System;
using System.Core;|}|}";

            await VerifyBlockSpansAsync(code,
                Region("span1", "// Foo ...", autoCollapse: true),
                Region("collapse2", "hint2", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestExternAliasesWithComments()
        {
            const string code = @"
$${|span1:// Foo
// Bar|}
{|hint2:extern {|collapse2:alias Foo;
extern alias Bar;|}|}";

            await VerifyBlockSpansAsync(code,
                Region("span1", "// Foo ...", autoCollapse: true),
                Region("collapse2", "hint2", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestWithComments()
        {
            const string code = @"
$${|span1:// Foo
// Bar|}";

            await VerifyBlockSpansAsync(code,
                Region("span1", "// Foo ...", autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestWithCommentsAtEnd()
        {
            const string code = @"
$${|hint1:using {|collapse1:System;|}|}
{|span2:// Foo
// Bar|}";

            await VerifyBlockSpansAsync(code,
                Region("collapse1", "hint1", CSharpStructureHelpers.Ellipsis, autoCollapse: true),
                Region("span2", "// Foo ...", autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        [WorkItem(539359, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/539359")]
        public async Task TestUsingKeywordWithSpace()
        {
            const string code = @"
$${|hint:using|} {|collapse:|}";

            await VerifyBlockSpansAsync(code,
                Region("collapse", "hint", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }
    }
}
