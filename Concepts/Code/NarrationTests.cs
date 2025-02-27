
namespace Odl.M10.QualityAssurance
{
    /// <summary>
    /// N0: Basics tests without any model data
    /// N1: Narration blueprint tests. No chapters are created
    /// N2: Narration population tests, with chapter definitions and chapters
    /// N3: Same as N2, but with discovery extraction as well (Discovery Output)
    /// </summary>
    public partial class NarrationTests : NarrationFixture, ITestCaseResultSink
    {
        #region Scaffolding

        protected const PopulationLevels OUTPUT_FILTER = PopulationLevels.All;

        protected DiscoveryDataChest<INarrationVariableBuilder> DataChest { get; set; }

        public override RenditionColumnAdapter DiscoveryColumnAdapter => new DiscoveryInjectionColumnAdapter(string.Empty);

        public NarrationTests()
        {
        }


        public override void Initialize()
        {
            Setup();
        }


        static bool _hasSubscription = false;

        [SetUp]
        public void Setup()
        {
            DynamicConstructs.ResetToDefaults();

            Observability.ResetToDefaults();
            Observability.Logging = ObservabilityPolicies.LogNever;
            Observability.Logging = ObservabilityPolicies.LogAlways;

            ResetRenditionStorage();
            Values.ResetCounter();
            HasSubject.DEBUG_COUNTER = 0;

            DataChest = new DiscoveryDataChest<INarrationVariableBuilder>();
            RenditionLogFormatter.RenderAttributesAsNodes = true;
            NarrationColumnAdapter.UseBlankMarkers = true;

            VariableBehavior.ResetToDefaults();
            VariableBehavior.CancelAllMuting = true;

            IdentityBag.UseFixedIdentities = true;
            IdentityBag.FixedIdentityWarning = true;
            IdentityBag.Reset();

            UseNewPackage();

            if (!_hasSubscription)
            {
                VariableIdentitites.RetrieveNewId += (sndr, args) =>
                {
                    var tp = sndr.GetType();
                    args.NewIdentity = IdentityBag.Next(tp.Name);
                };

                _hasSubscription = true;
            }
        }

        #endregion

        #region Helpers
        private static void UseNewPackage(bool muteErrors = false)
        {
            VariableBehavior.Actions.CollisionHandling_IsGen3 = true;
            VariableBehavior.Actions.SupressDistinctIdPattern = true;
            VariableBehavior.Display.UseFootprintIdInsteadOfRecipeId = true;
            VariableBehavior.Actions.UseNewStyleRenditionFormatting = true;
            VariableBehavior.Actions.DisableCircularReferenceCheck = true;
            IdTag.OmitBraces = true;

            if (!muteErrors)
            {
                VariableBehavior.CancelAllMuting = false;
                VariableBehavior.Consequences.MuteUncommitedTokensException = true;
                VariableBehavior.Consequences.MuteExceptionOnAnomalies = true;
                VariableBehavior.Actions.SubstituteMissingNeutralName = true;
            }
        }

        #endregion

        #region Composition

        private static ChartSetup<SampleUniChart> StandardChart(ChartComposer<SampleUniChart> composer)
        {
            return composer.For(chart => chart.NA1).XVarPipeline_In1Out2_ClsItf()
                                                   .ConfigSink(a1 => a1.X1).Simple()
                           .For(chart => chart.NMyR2_Entity_IN2).XVarPipeline_In2Out3()
                                                                .ConfigSink(ent => ent.X1).Simple()
                                                                .ConfigSink(ent => ent.X2).WithLinkTo(shart => shart.NA1, a1 => a1.Output.Dyn1)

                           .For(chart => chart.NMyR2_Entity_IN2, ent => ent.Dyn2DRCond2A, Labels.Conditions.Fields)
                                                                                       .ConfigSink(fds => fds.X1).Simple()
                                                                                       .ConfigSink(fds => fds.X2).WithLinkTo(shart => shart.NA1, a1 => a1.Output.Dyn1)
                                                                                       .ConfigSource(ffs => ffs.Dyn1).Simple(x => x.X1)
                                                                                       .ConfigSource(ffs => ffs.Dyn2).Simple(x => x.X2)
                           .For(chart => chart.NMyR2_Entity_IN2, ent => ent.Dyn2DRCond2B, Labels.Conditions.Links)
                                                                                       .ConfigSink(fds => fds.X1).Simple()
                                                                                       .ConfigSink(fds => fds.X2).WithLinkTo(shart => shart.NA1, a1 => a1.Output.Dyn1)
                                                                                       .ConfigSource(ffs => ffs.Dyn1).Simple(x => x.X1)
                                                                                       .ConfigSource(ffs => ffs.Dyn2).Simple(x => x.X2)
                           .Publish();
        }


        private static ChartSetup<SampleUniChart> ConstructorChart(ChartComposer<SampleUniChart> composer)
        {
            return composer.For(chart => chart.NA1).ConfigSink(a1 => a1.X1).Simple()
                                                   .ConfigSource(a1 => a1.Dyn1).Simple()
                                                   .ConfigSource(a1 => a1.Dyn2).Unary(r => r.X1, OutputVariableValues.PublishAsInterface)
                           .For(chart => chart.NStrEntity).ConfigSink(ent => ent.X1).Simple()
                                                       .ConfigSource(outp => outp.Dyn1, Labels.Entity.EntityName).Simple(r => r.X1)
                                                       .ConfigSource(outp => outp.Dyn2, Labels.Entity.EntityInterfaceName).Simple(r => r.X1)
                                                       .ConfigSource(outp => outp.Dyn3, Labels.Entity.EntityOuter).Simple(r => r.X1)
                                                       .ConfigSource(outp => outp.Dyn4, Labels.Entity.EntityInner).Unary(r => r.X1, OutputVariableValues.PublishAsInterface)
                                                       .ConfigSource(outp => outp.Dyn5, Labels.Entity.T2_F5).Simple(r => r.X1)
                           .For(chart => chart.NStrEntity, ent => ent.Dyn2DRCond2A, Labels.Entity.CompositeLinksToRoot)
                                                             .ConfigSink(fds => fds.X1).Simple()
                                                             .ConfigSink(fds => fds.X2).WithLinkTo(shart => shart.NA1, a1 => a1.Output.Dyn1)
                                                             .ConfigSource(src => src.Dyn1).Simple(x => x.X1)
                                                             .ConfigSource(src => src.Dyn2).WithLinkFrom(x => x.X2)

                           .For(chart => chart.NStrEntity, ent => ent.Dyn1RCond2A, Labels.Entity.RemoteLinksToRoot)
                                                             .ConfigSink(fds => fds.X1).WithLinkTo(shart => shart.NA1, a1 => a1.Output.Dyn1)
                                                             .ConfigSource(src => src.Dyn1).Simple(x => x.X1)
                                                             .ConfigSource(src => src.Dyn2).WithLinkFrom(x => x.X1)

                           .For(chart => chart.NStrEntity, ent => ent.Dyn1DCond2A, Labels.Entity.LocalLinksToRoot)
                                                             .ConfigSink(fds => fds.X1).Simple()
                                                             .ConfigSource(src => src.Dyn1).Simple(x => x.X1)
                                                             .ConfigSource(src => src.Dyn2).Simple(x => x.X1)
                           .Publish();
        }



        private static ChartSetup<SampleUniChart> MinimalChart(ChartComposer<SampleUniChart> composer)
        {
            return composer.For(chart => chart.NA1).ConfigSink(a1 => a1.X1).Simple()
                                                   .ConfigSource(a1 => a1.Dyn1).Simple(x => x.X1)
                                                   .ConfigSource(a1 => a1.Dyn2).Unary(x => x.X1, OutputVariableValues.PublishAsInterface)
                           .Publish();
        }


        private static void NoBlueprint(SampleUniChart chart, INarrationRepository<SampleUniChart> repo)
        {
            // Use this to do nothing during blueprint step
        }

        #endregion

        #region Reuseables

        private IList<IDiscoveryDefinitionNode<INarrationVariableBuilder>> Evaluate(TestResultsRecord<INarrationVariableBuilder> result)
        {
            var body = result.DataRoot.Probe(DiscoveryProbe.Single[NT.InstanceTypes.Book]
                                            .Single[NT.InstanceTypes.Chapter]
                                            .Single[NT.InstanceTypes.TypeDefinition]
                                            .All[NT.InstanceTypes.Body]).ToList();
            return body;
        }


        public TestResultsRecord<INarrationVariableBuilder> RunConstructorNEW(string name,
           Action<IClassDefinition<VarPipelineD1<SampleUniChart, OutputMember5L, IDirectChapterParameter>>> onConstruct, bool addFailing = false)
        {
            TestResultsRecord<INarrationVariableBuilder> result = null;

            var setup = NarrationTestCaseSetup.Create<SampleUniChart>();
            var output = setup.CreateContainer(this);

            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var chapters = new List<IChapter>();
                test.Run(
                    ConstructorChart,
                    (recipe, repo) =>
                    {
                        var docChapter = repo.CreateChapterDefinition(recipe.NStrEntity);
                        var docClass = docChapter.AddClass(c => c.Output.Dyn1, ND.Str("Entity.Class"));

                        // Test dependent
                        onConstruct(docClass);
                    },
                    (recipe, context) =>
                    {
                        var d1 = context.CreateScope(recipe.NStrEntity.CreateSimple(I.R2.R001));
                        var docu1 = d1.CreateChapter(S.Create("UserSpecification"));

                        if (addFailing)
                        {
                            var d2 = context.CreateScope(recipe.NStrEntity.CreateSimple(I.R2.R002));
                            var docu2 = d2.CreateChapter(S.Create("UserGroupSpecification"));
                        }

                        chapters.AddRange(context.AllChapters);
                    },
                    (root) =>
                    {
                        result = TestResultsRecord.New(chapters, root);
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(1));

            return result;
        }


        #endregion


        [Test(Description = "Basic sanity check of one chapter/class. Implicit pipline defs")]
        public void N3M_C1_EmptyClsImpl_Lvl0()
        {
            var IDEN1 = "ValueR01";
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var emitData = SetupPublishing(test);
                test.Run(
                    (recipe, repo) =>
                    {
                        repo.CreateChapterDefinition(recipe.NA1).AddClass(c => c.Output.Dyn1);
                    },
                    (recipe, context) =>
                    {
                        var id = ScopeIdentity.Create(IDEN1);
                        context.CreateScope(recipe.NA1.CreateSimple(id.Next())).CreateChapter(S.Create(IDEN1));
                    }, emitData.Hook);
            }

            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition), Is.EqualTo(1));
            var idList = output.InDiscOutputIdentifiers(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition)
                               .Columns(DiscoveryColumns.IdentityString);
            Assert.That(idList[0], Is.EqualTo(IDEN1));
        }


        [Test(Description = "Basic sanity check of one chapter/class. Explicit pipeline defs")]
        public void N3M_C1_EmptyClsExpl_Lvl0()
        {
            var IDEN1 = "ValueR01";
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var emitData = SetupPublishing(test);
                test.Run(
                    MinimalChart,
                    (recipe, repo) =>
                    {
                        repo.CreateChapterDefinition(recipe.NA1).AddClass(c => c.Output.Dyn1);
                    },
                    (recipe, context) =>
                    {
                        var id = ScopeIdentity.Create(IDEN1);
                        context.CreateScope(recipe.NA1.CreateSimple(id.Next())).CreateChapter(S.Create(IDEN1));
                    }, emitData.Hook);
            }

            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition), Is.EqualTo(1));
            var idList = output.InDiscOutputIdentifiers(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition)
                               .Columns(DiscoveryColumns.IdentityString);
            Assert.That(idList[0], Is.EqualTo(IDEN1));
        }


        [Test(Description = "Dynamic chart with repeated/identical pipeline definitions should not be allowed")]
        public void X1M_C0_DuplicatedConfig_Lvl0()
        {
            var ex = Assert.Throws<DynamicRecipeException>(() =>
            {
                var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
                var output = setup.CreateContainer(this);
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    var emitData = SetupPublishing(test);
                    test.Run(
                        (setup) =>
                        {
                            // Level 1: Blueprint
                            return setup.For(rec => rec.NA1).ConfigSink(r => r.X1).Simple()
                                        .For(rec => rec.NA1).ConfigSink(r => r.X1).Simple().Publish();
                        });
                }
            });
            Assert.That(ex.Message, Is.EqualTo(NarrationErrors.Messages.ERR_DYN_FEED_NOT_UNIQUE));
        }


        [Test(Description = "Trying to create chapter without chapter definition")]
        public void X3M_C1_NoChapterDef_Lvl0()
        {
            var IDEN1 = "ValueR01";
            var ex = Assert.Throws<MissingConfigException>(() =>
            {
                var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
                var output = setup.CreateContainer(this);
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    var emitData = SetupPublishing(test);
                    test.Run(MinimalChart, NoBlueprint,
                        (recipe, context) =>
                        {
                            var id = ScopeIdentity.Create(IDEN1);
                            context.CreateScope(recipe.NA1.CreateSimple(id.Next())).CreateChapter(S.Create(IDEN1));
                        }, emitData.Hook);
                }
            });
            Assert.That(ex.Message, Is.EqualTo(NarrationErrors.Messages.ERR_CHAPTER_NOT_CONFIGURED));
        }


        [Test(Description = "Dynamic output is missing configuration")]
        public void X1M_C0_MissingOutputDef_Illegal()
        {
            var ex = Assert.Throws<DynamicRecipeException>(() =>
            {
                var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
                var output = setup.CreateContainer(this);
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    var emitData = SetupPublishing(test);
                    test.Run(
                        (composer) =>
                        {
                            var res = composer
                                        .For(cht => cht.D2_3).ConfigSink(s => s.X1).Simple()
                                                             .ConfigSink(s => s.X2).Simple()
                                                             .ConfigSource(s => s.Dyn1).Simple(r => r.X1)
                                                             //.ConfigSource(s => s.Dyn3).Simple(r => r.X1)
                                                             .ConfigSource(s => s.Dyn2).Unary(r => r.X2, OutputVariableValues.PublishAsInterface);

                            return res.Publish();
                        });
                }
            });
            Assert.That(ex.ErrCode, Is.EqualTo(NarrationErrors.ErrCodes.eDynamicIncomplete));
        }


        [Test(Description = "Wrong input type")]
        public void X1M_C1_MisalignedInputs_Illegal()
        {
            var ex = Assert.Throws<NarrationException>(() =>
            {
                var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
                var output = setup.CreateContainer(this);
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    test.Run(
                        (composer) =>
                        {
                            var res = composer
                                        .For(cht => cht.C1D_1).ConfigSink(s => s.X1).Simple()
                                                             .ConfigSource(s => s.Dyn1).Simple(r => r.X1)
                                        .For(cht => cht.R1_2).ConfigSink(s => s.X1).Simple()
                                                             .ConfigSource(s => s.Dyn1).Simple(r => r.X1);

                            return res.Publish();
                        },
                        (recipe, repo) =>
                        {
                            repo.CreateChapterDefinition(recipe.C1D_1);
                            repo.CreateChapterDefinition(recipe.R1_2);
                        });
                }
            });
            Assert.That(ex.ErrCode, Is.EqualTo(NarrationErrors.ErrCodes.eMismatchInputType));
        }



        [Test(Description = "Chained pipeline value in one level")]
        public void N3M_C1_EmptyClass_Lvl1()
        {
            var IDEN1 = "ValueR01";
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var emitData = SetupPublishing(test);
                test.Run(
                    (setup) =>
                    {
                        return setup.For(rec => rec.NA1).XVarPipeline_In1Out2_ClsItf()
                                                        .ConfigSink(r => r.X1).Simple()
                                    .For(rec => rec.NR1).ConfigSink(r => r.X1).WithLinkTo(c => c.NA1, c => c.Output.Dyn1)
                                                        .ConfigSource(r => r.Dyn1).Simple(x => x.X1)
                                    .Publish();
                    },
                    (recipe, repo) =>
                    {
                        repo.CreateChapterDefinition(recipe.NA1);
                        repo.CreateChapterDefinition(recipe.NR1).AddClass(c => c.Output.Dyn1);
                    },
                    (recipe, context) =>
                    {
                        var id = ScopeIdentity.Create(IDEN1);
                        var c1 = context.CreateScope(recipe.NA1.CreateSimple(id.Next())).CreateChapter(S.Create(IDEN1));
                        var c2 = context.CreateScope(recipe.NR1.CreateSimple(id.Next())).CreateChapter(R.Create(c1));
                    }, emitData.Hook);
            }

            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition), Is.EqualTo(1));
            var idList = output.InDiscOutputIdentifiers(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition)
                               .Columns(DiscoveryColumns.IdentityString);
            Assert.That(idList[0], Is.EqualTo(IDEN1));
        }


        [Test(Description = "One chapter/class with multi variable input")]
        public void N3M_C2_EmptyClass_Lvl0()
        {
            var IDEN1 = "ValueR01";
            var IDEN2 = "ValueR02";
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var emitData = SetupPublishing(test);
                test.Run(
                    (setup) =>
                    {
                        return setup.For(rec => rec.ND1).ConfigSink(r => r.X1).Simple().ConfigSink(r => r.X2).Simple()
                                                        .ConfigSource(r => r.Dyn1).Multinary(x => x.X1, x => x.X2, OutputVariableValues.Merge)
                                                        .Publish();
                    },
                    (recipe, repo) =>
                    {
                        repo.CreateChapterDefinition(recipe.ND1).AddClass(c => c.Output.Dyn1);
                    },
                    (recipe, context) =>
                    {
                        var id = ScopeIdentity.Create(IDEN1);
                        context.CreateScope(recipe.ND1.CreateSimple(id.Next())).CreateChapter(S.Create(IDEN1), S.Create(IDEN2));
                    }, emitData.Hook);
            }

            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition), Is.EqualTo(1));
            var idList = output.InDiscOutputIdentifiers(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition)
                               .Columns(DiscoveryColumns.IdentityString);
            Assert.That(idList[0], Is.EqualTo(IDEN1 + "_" + IDEN2));

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(1));
            Assert.That(output.InDiscOutput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.TypeDefinition), Is.EqualTo(1));
        }


        [Test(Description = "One chapter with class and interface attached")]
        public void N3M_C1_EmptyClassItf_Lvl0()
        {
            var IDEN1 = "ValueR01";
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var emitData = SetupPublishing(test);
                test.Run(
                    MinimalChart,
                    (recipe, repo) =>
                    {
                        var cd = repo.CreateChapterDefinition(recipe.NA1);
                        cd.AddClass(c => c.Output.Dyn1);
                        cd.AddInterface(c => c.Output.Dyn2);
                    },
                    (recipe, context) =>
                    {
                        var id = ScopeIdentity.Create(IDEN1);
                        context.CreateScope(recipe.NA1.CreateSimple(id.Next())).CreateChapter(S.Create(IDEN1));
                    }, emitData.Hook);
            }

            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition), Is.EqualTo(2));
            var idList = output.InDiscOutputIdentifiers(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition)
                               .Columns(DiscoveryColumns.IdentityString);
            Assert.That(idList[0], Is.EqualTo("I" + IDEN1));
            Assert.That(idList[1], Is.EqualTo(IDEN1));
        }


        [Test(Description = "Two instances with same identifier should be collision handled")]
        public void N3M_C3_IdenticalNames_Lvl0()
        {
            var IDEN1 = "ValueR01";
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var emitData = SetupPublishing(test);
                test.Run(
                    MinimalChart,
                    (recipe, repo) =>
                    {
                        repo.CreateChapterDefinition(recipe.NA1).AddClass(c => c.Output.Dyn1);
                    },
                    (recipe, context) =>
                    {
                        var id = ScopeIdentity.Create(IDEN1);
                        context.CreateScope(recipe.NA1.CreateSimple(id.Next())).CreateChapter(S.Create(IDEN1));
                        context.CreateScope(recipe.NA1.CreateSimple(id.Next())).CreateChapter(S.Create(IDEN1));
                    }, emitData.Hook);
            }

            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition), Is.EqualTo(2));
            var idList = output.InDiscOutputIdentifiers(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition)
                               .Columns(DiscoveryColumns.IdentityString);
            Assert.That(idList[0], Is.EqualTo(TokenResolver.Subst(IDEN1, 1)));
            Assert.That(idList[1], Is.EqualTo(TokenResolver.Subst(IDEN1, 2)));
        }


        [Test]
        [NarrationRendition]
        public void N1M_C6_CtorField1Arg()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);

            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    MinimalChart,
                    (recipe, repo) =>
                    {
                        var chapter = repo.CreateChapterDefinition(recipe.NA1);

                        var myClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                        myClass.AddDecoration(MemberFactory.Constructor)
                               .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn1);
                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
        }


        [Test]
        [NarrationRendition]
        public void X1M_C6_CommonDuplicates_Illegal()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);

            var ex = Assert.Throws<InvalidConfigException>(() =>
            {
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    test.Run(
                        MinimalChart,
                        (recipe, repo) =>
                        {
                            var chapter = repo.CreateChapterDefinition(recipe.NA1);

                            var myClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                            myClass.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplateA), c => c.Output.Dyn1, c => c.Output.Dyn1);
                            myClass.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplateA), c => c.Output.Dyn1, c => c.Output.Dyn1);
                        },
                        renditions: ERenditionRequests.eBlueprint);
                }
            });

            Assert.That(ex.Message, Is.EqualTo(NarrationErrors.Messages.ERR_DECORATION_DUPLICATE));
        }


        [Test]
        [NarrationRendition]
        public void X2M_C1_UndeclaredChapDef_Illegal()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);

            var ex = Assert.Throws<MissingConfigException>(() =>
            {
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    test.Run(
                        (setup) =>
                        {
                            return setup.For(rec => rec.NA1).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple()
                                        .For(rec => rec.NA2).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple()
                                        .Publish();
                        },
                        (recipe, repo) =>
                        {
                            var chapter = repo.CreateChapterDefinition(recipe.NA1);
                            var myClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                        },
                        (recipe, context) =>
                        {
                            var e1 = context.CreateScope(recipe.NA2.CreateSimple(I.R2.R001));
                            var document1 = e1.CreateChapter(S.Create("MyDocument_01"));
                        });
                }
            });

            Assert.That(ex.Message, Is.EqualTo(NarrationErrors.Messages.ERR_CHAPTER_NOT_CONFIGURED));
        }


        [Test]
        [NarrationRendition]
        public void X2M_C1_DuplicateChapter_Illegal()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);

            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    test.Run(
                        (setup) =>
                        {
                            return setup.For(rec => rec.NA1).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple()
                                        .For(rec => rec.NA2).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple().Publish();
                        },
                        (recipe, repo) =>
                        {
                            var chapter = repo.CreateChapterDefinition(recipe.NA1);
                            var myClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                        },
                        (recipe, context) =>
                        {
                            var e1 = context.CreateScope(recipe.NA1.CreateSimple(I.R2.R001));
                            var document1 = e1.CreateChapter(S.Create("MyDocument_01"));
                            var document2 = e1.CreateChapter(S.Create("MyDocument_02"));
                        });
                }
            });

            Assert.That(ex.Message, Is.EqualTo(NarrationErrors.Messages.ERR_CHAPTER_DUPLICATE));
        }


        [Test]
        [NarrationRendition]
        public void X2M_C1_DuplicateClasses_Illegal()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);

            var ex = Assert.Throws<InvalidConfigException>(() =>
            {
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    test.Run(
                        (setup) =>
                        {
                            return setup.For(rec => rec.NA1).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple()
                                        .For(rec => rec.NA2).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple().Publish();
                        },
                        (recipe, repo) =>
                        {
                            var chapter = repo.CreateChapterDefinition(recipe.NA1);
                            var myClass1 = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                            var myClass2 = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                        });
                }
            });

            Assert.That(ex.Message, Is.EqualTo(NarrationErrors.Messages.ERR_CLASS_NOT_UNIQUE));
        }


        [Test]
        [NarrationRendition]
        public void X1M_C6_ConditionalDuplicates_Illegal()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);

            var ex = Assert.Throws<InvalidConfigException>(() =>
            {
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    test.Run(
                        (setup) =>
                        {
                            return setup.For(rec => rec.NA1).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple()
                                        .For(rec => rec.NA2).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple()
                                        .For(rec => rec.NA1, r => r.Dyn2DRCond2A).ConfigSink(r => r.X2).WithLinkTo(rec => rec.NA2, a2 => a2.Output.Dyn1)
                                                                                 .ConfigSink(r => r.X1).Simple()
                                                                                 .ConfigSource(r => r.Dyn1).Simple(x => x.X1)
                                                                                 .ConfigSource(r => r.Dyn2).WithLinkFrom(x => x.X2)
                                        .Publish();
                        },
                        (recipe, repo) =>
                        {
                            var chapter = repo.CreateChapterDefinition(recipe.NA1);

                            var etyClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                            etyClass.OnlyFor(c => c.Dyn2DRCond2A, m => m.Father)
                                    .AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplateA), c => c.Output.Dyn1, c => c.Output.Dyn1);
                            etyClass.OnlyFor(c => c.Dyn2DRCond2A, m => m.Father)
                                    .AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplateA), c => c.Output.Dyn1, c => c.Output.Dyn1);
                        },
                        renditions: ERenditionRequests.eBlueprint);
                }
            });

            Assert.That(ex.Message, Is.EqualTo(NarrationErrors.Messages.ERR_DECORATION_DUPLICATE));
        }


        [Test]
        [NarrationRendition]
        public void A1_N1M_C6_ConditionalTwins_Accepted()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (setup) =>
                    {
                        return setup.For(rec => rec.NA1).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple()
                                        .For(rec => rec.NA2).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple()
                                        .For(rec => rec.NA1, r => r.Dyn2DRCond2A).ConfigSink(r => r.X2).WithLinkTo(rec => rec.NA2, a2 => a2.Output.Dyn1)
                                                                                 .ConfigSink(r => r.X1).Simple()
                                                                                 .ConfigSource(r => r.Dyn1).Simple(x => x.X1)
                                                                                 .ConfigSource(r => r.Dyn2).WithLinkFrom(x => x.X2)
                                    .Publish();
                    },
                    (recipe, repo) =>
                    {
                        var chapter = repo.CreateChapterDefinition(recipe.NA1);

                        var etyClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                        etyClass.OnlyFor(c => c.Dyn2DRCond2A, m => m.Father)
                                .AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplateA), c => c.Output.Dyn1, c => c.Output.Dyn1);
                        etyClass.OnlyFor(c => c.Dyn2DRCond2A, m => m.Father)
                                .AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplateA), c => c.Output.Dyn2, c => c.Output.Dyn2);
                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
        }


        [Test]
        [NarrationRendition]
        public void N1M_C6_CtorField2Arg()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);

            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    MinimalChart,
                    (recipe, repo) =>
                    {
                        var chapter = repo.CreateChapterDefinition(recipe.NA1);

                        var myClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                        myClass.AddDecoration(MemberFactory.Constructor)
                               .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn1)
                               .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn1);
                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
        }


        [Test(Description = "One class with constructor from multi-dimensional pipeline")]
        [NarrationRendition]
        public void N1M_C6_CtorFieldRelated()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);

            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var emitData = SetupPublishing(test);
                test.Run(
                    (setup) =>
                    {
                        return setup.For(rec => rec.NA1).XVarPipeline_In1Out2_ClsItf().ConfigSink(r => r.X1).Simple()
                                    .For(rec => rec.NMyR2_Entity_IN2).ConfigSink(r => r.X1).Simple()
                                                        .ConfigSink(r => r.X2).WithLinkTo(r => r.NA1, a1 => a1.Output.Dyn1)
                                                        .ConfigSource(r => r.Dyn1).Simple(x => x.X1)
                                                        .ConfigSource(r => r.Dyn2).Multinary(x => x.X1, x => x.X2, OutputVariableValues.Merge)
                                                        .ConfigSource(r => r.Dyn3).Simple(x => x.X2)
                                                       .Publish();
                    },
                    (recipe, repo) =>
                    {
                        var chapter = repo.CreateChapterDefinition(recipe.NMyR2_Entity_IN2);

                        var myClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));
                        myClass.AddDecoration(MemberFactory.Constructor)
                               .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn3);
                        myClass.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplateA), c => c.Output.Dyn2, c => c.Output.Dyn2);
                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
        }



        [Test]
        public void N3M_C5_2Cond1Trigged()
        {
            const string X_ROOT = "MyRoot";
            const string X_DOCU = "MyDocu";
            const string X_FIELD = "Medlemsnamn";
            var subRoot = DataChest.SubscribeToBlocks(X_ROOT, NT.InstanceTypes.TypeDefinition);
            var subDocu = DataChest.SubscribeToBlocks(X_DOCU, NT.InstanceTypes.TypeDefinition);

            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var emitData = SetupPublishing(test);
                test.Run(
                    (composer) =>
                    {
                        var setup = composer.For(chart => chart.NA1).XVarPipeline_In1Out2_ClsItf()
                                                                    .ConfigSink(pip => pip.X1).Simple()
                                            .For(chart => chart.NB1).XVarPipeline_In1Out2_ClsItf()
                                                                    .ConfigSink(pip => pip.X1).Simple()

                                            .For(chart => chart.NB1, b1 => b1.Dyn2DRCond2B).ConfigSink(lnk => lnk.X1).Simple()
                                                                                           .ConfigSink(lnk => lnk.X2).WithLinkTo(rec => rec.NA1, s1 => s1.Output.Dyn1)
                                                                                           .ConfigSource(src => src.Dyn1).Simple(x => x.X1)
                                                                                           .ConfigSource(src => src.Dyn2).WithLinkFrom(x => x.X2)
                                            .For(chart => chart.NB1, b1 => b1.Dyn2DRCond2A).ConfigSink(lnk => lnk.X1).Simple()
                                                                                           .ConfigSink(lnk => lnk.X2).WithLinkTo(rec => rec.NA1, s1 => s1.Output.Dyn1)
                                                                                           .ConfigSource(src => src.Dyn1).Simple(x => x.X1)
                                                                                           .ConfigSource(src => src.Dyn2).WithLinkFrom(x => x.X2)
                                            .Publish();
                        return setup;
                    },
                    (recipe, repo) =>
                    {
                        var s1 = repo.CreateChapterDefinition(recipe.NA1);
                        var sc1 = s1.AddClass(pip => pip.Output.Dyn1, ND.Str("Root.Class"));

                        var s2 = repo.CreateChapterDefinition(recipe.NB1);
                        var sc2 = s2.AddClass(pip => pip.Output.Dyn1, ND.Str("Entity.Class"));

                        sc2.OnlyFor(c => c.Dyn2DRCond2A, m => m.Father)
                                    .AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplateA), r => r.Output.Dyn1, r => r.Output.Dyn2);
                    },
                    (recipe, context) =>
                    {
                        var id = ScopeIdentity.Create("ID1");

                        var r1 = context.CreateScope(recipe.NA1.CreateSimple(id.Next()));
                        var root1 = r1.CreateChapter(S.Create(X_ROOT));

                        var d1 = context.CreateScope(recipe.NB1.CreateSimple(id.Next()));
                        var docu1 = d1.CreateChapter(S.Create(X_DOCU));

                        docu1.For(c => c.Dyn2DRCond2A, r => r.Father).Create(S.Create(X_FIELD), R.Create(root1, "Doc-->Root"));
                    }, emitData.Hook);
            }

            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition), Is.EqualTo(2));
            var idList = output.InDiscOutputIdentifiers(DiscoveryColumns.NarrationTag, NT.InstanceTypes.TypeDefinition)
                               .Columns(DiscoveryColumns.IdentityString);
            Assert.That(idList[0], Is.EqualTo(X_ROOT));
            Assert.That(idList[1], Is.EqualTo(X_DOCU));

            Assert.That(subRoot.TotalCount, Is.EqualTo(1));
            Assert.That(subDocu.TotalCount, Is.EqualTo(1));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(2));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(2));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(2));
        }


        [Test]
        [NarrationRendition]
        public async Task EM01_XX_C_F1F2_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    StandardChart,
                    (recipe, repo) =>
                    {
                        var entityIn2 = repo.CreateChapterDefinition(recipe.NMyR2_Entity_IN2);

                        // Dekorera med länkar, både på klassen och interfacet
                        var bc2 = entityIn2.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));             // Endast rotens värde

                        var bi2 = entityIn2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF"));
                        var bi21 = bi2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF.INTF"));

                        var clsAggregation = bc2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Father, Labels.Conditions.Aggregation);
                        clsAggregation.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn1, c => c.Output.Dyn1);

                        var itfAggregation = bi2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Father, Labels.Conditions.Aggregation);
                        itfAggregation.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn2, c => c.Output.Dyn2);

                        var itfInheritance = bi2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Mother, Labels.Conditions.Inheritance);
                        itfInheritance.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn2, c => c.Output.Dyn2);          // #######################
                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(3));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
        }


        [Test]
        [NarrationRendition]
        public async Task EM06_XX_X_F1F2_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    StandardChart,
                    (recipe, repo) =>
                    {
                        var entityIn2 = repo.CreateChapterDefinition(recipe.NMyR2_Entity_IN2);

                        // Dekorera med länkar, både på klassen och interfacet
                        var bc2 = entityIn2.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));             // Endast rotens värde

                        var bi2 = entityIn2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF"));
                        var bi21 = bi2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF.INTF"));

                        var itfAggregation = bi2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Father, Labels.Conditions.Aggregation);
                        itfAggregation.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn2, c => c.Output.Dyn2);

                        var itfInheritance = bi2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Mother, Labels.Conditions.Inheritance);
                        itfInheritance.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn2, c => c.Output.Dyn2);
                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(3));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
        }


        [Test]
        [NarrationRendition]
        public async Task EM07_XX_X_F1XX_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    StandardChart,
                    (recipe, repo) =>
                    {
                        var entityIn2 = repo.CreateChapterDefinition(recipe.NMyR2_Entity_IN2);

                        // Dekorera med länkar, både på klassen och interfacet
                        var bc2 = entityIn2.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));             // Endast rotens värde

                        var bi2 = entityIn2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF"));
                        var bi21 = bi2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF.INTF"));

                        var itfAggregation = bi2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Father, Labels.Conditions.Aggregation);
                        itfAggregation.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn2, c => c.Output.Dyn2);
                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(3));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
        }


        [Test]
        [NarrationRendition]
        public async Task EM04_XX_C_F1XX_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    StandardChart,
                    (recipe, repo) =>
                    {
                        var entityIn2 = repo.CreateChapterDefinition(recipe.NMyR2_Entity_IN2);

                        // Dekorera med länkar, både på klassen och interfacet
                        var bc2 = entityIn2.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));             // Endast rotens värde

                        var bi2 = entityIn2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF"));
                        var bi21 = bi2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF.INTF"));

                        var clsAggregation = bc2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Father, Labels.Conditions.Aggregation);
                        clsAggregation.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn1, c => c.Output.Dyn1);

                        var itfAggregation = bi2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Father, Labels.Conditions.Aggregation);
                        itfAggregation.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn2, c => c.Output.Dyn2);

                        var itfInheritance = bi2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Mother, Labels.Conditions.Inheritance);
                        itfInheritance.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn2, c => c.Output.Dyn2);          // #######################

                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(3));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
        }


        [Test]
        [NarrationRendition]
        public async Task EM05_XX_C_XXF2_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    StandardChart,
                    (recipe, repo) =>
                    {
                        var entityIn2 = repo.CreateChapterDefinition(recipe.NMyR2_Entity_IN2);

                        // Dekorera med länkar, både på klassen och interfacet
                        var bc2 = entityIn2.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));             // Endast rotens värde

                        var bi2 = entityIn2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF"));
                        var bi21 = bi2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF.INTF"));

                        var clsAggregation = bc2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Father, Labels.Conditions.Aggregation);
                        clsAggregation.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn1, c => c.Output.Dyn1);

                        var itfInheritance = bi2.OnlyFor(c => c.Dyn2DRCond2B, f => f.Mother, Labels.Conditions.Inheritance);
                        itfInheritance.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn2, c => c.Output.Dyn2);          // #######################
                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(3));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
        }


        [Test]
        [NarrationRendition]
        public async Task EM08_XX_X_XXXX_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>();
            var output = setup.CreateContainer(this);

            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    StandardChart,
                    (recipe, repo) =>
                    {
                        var entityIn2 = repo.CreateChapterDefinition(recipe.NMyR2_Entity_IN2);

                        // Dekorera med länkar, både på klassen och interfacet
                        var bc2 = entityIn2.AddClass(c => c.Output.Dyn1, ND.Str("T2.CLASS"));             // Endast rotens värde

                        var bi2 = entityIn2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF"));
                        var bi21 = bi2.AddInterface(c => c.Output.Dyn3, ND.Str("T2.INTF.INTF"));
                    },
                    renditions: ERenditionRequests.eBlueprint);
            }

            Assert.That(output.NarrationRendition.Children.First().Children.First().Children.Count, Is.EqualTo(1));
        }

        #region Constructors

        [Test]
        [NarrationRendition]
        [DiscoveryRendition]
        public void CC01_N3_Ctor1_1OneScalarArg_NEW()
        {
            var res = RunConstructorNEW(nameof(CC01_N3_Ctor1_1OneScalarArg_NEW), docClass =>
            {
                // Blank ctor()
                var constructor = docClass.AddDecoration(MemberFactory.Constructor);
                // Calling the base class
                constructor.AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn1);
            });

            var body = Evaluate(res);
            Assert.That(body.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.First.ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.First.ChildElements.First.ChildElements.Count, Is.EqualTo(1));
        }


        [Test]
        [NarrationRendition]
        [DiscoveryRendition]
        public void CC02_N3_Ctor1_2OneVectorialArg_NEW()
        {
            var res = RunConstructorNEW(nameof(CC02_N3_Ctor1_2OneVectorialArg_NEW), docClass =>
            {
                // Blank ctor()
                var constructor2 = docClass.AddDecoration(MemberFactory.Constructor);
                // Calling the base class
                constructor2.AddInvokationArgument(SampleUniChart.TypeTemplateB, c => c.Output.Dyn1, c => c.Output.Dyn3);
            });

            var body = Evaluate(res);
            Assert.That(body.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.First.ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.First.ChildElements.First.ChildElements.Count, Is.EqualTo(2));
        }


        [Test]
        [NarrationRendition]
        [DiscoveryRendition]
        public void CC03_N3_Ctor1_3ThreeArgs_NEW()
        {
            var res = RunConstructorNEW(nameof(CC03_N3_Ctor1_3ThreeArgs_NEW), docClass =>
            {
                var tp = docClass.GetType().Name;
                // Blank ctor()                                                                     // #######################################################
                var constructor2 = docClass.AddDecoration(MemberFactory.Constructor);               // Creation of sticky arguments to match in/fwd arguments? 
                // Calling the base class
                constructor2
                            .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn4)
                            .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn3)
                            .AddInvokationArgument(SampleUniChart.TypeTemplateB, c => c.Output.Dyn1, c => c.Output.Dyn3);
            });

            var body = Evaluate(res);
            Assert.That(body.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.First.ChildElements.Count, Is.EqualTo(3));
            Assert.That(body.First().AttributeElements.First.ChildElements[0].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.First.ChildElements[1].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.First.ChildElements[2].ChildElements.Count, Is.EqualTo(2));
        }


        [Test]
        [NarrationRendition]
        [DiscoveryRendition]
        public void CC04_N3_Ctor1_3TwoArgs_NEW()
        {
            var res = RunConstructorNEW(nameof(CC04_N3_Ctor1_3TwoArgs_NEW), docClass =>
            {
                // Blank ctor()                                                                     // #######################################################
                var constructor2 = docClass.AddDecoration(MemberFactory.Constructor);               // Creation of sticky arguments to match in/fwd arguments? 
                // Calling the base class
                constructor2.AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn4)
                            .AddInvokationArgument(SampleUniChart.TypeTemplateB, c => c.Output.Dyn1, c => c.Output.Dyn3);
            });

            var body = Evaluate(res);
            Assert.That(body.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.First.ChildElements.Count, Is.EqualTo(2));
            Assert.That(body.First().AttributeElements.First.ChildElements[0].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.First.ChildElements[1].ChildElements.Count, Is.EqualTo(2));
        }


        [Test]
        [NarrationRendition]
        [DiscoveryRendition]
        public void CC05_N3_Ctor2_0OneScalarArgIden_NEW()
        {
            var res = RunConstructorNEW(nameof(CC05_N3_Ctor2_0OneScalarArgIden_NEW), docClass =>
            {
                // Blank ctor()
                var constructor = docClass.AddDecoration(MemberFactory.Constructor);
                // Calling the base class
                constructor.AddForwardingArgument(c => c.Output.Dyn1)
                           .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn1);
            });

            var body = Evaluate(res);
            Assert.That(body.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.Count, Is.EqualTo(2));
            Assert.That(body.First().AttributeElements[0].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements[1].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements[0].ChildElements.First.ChildElements.Count, Is.EqualTo(1));
        }


        [Test]
        [NarrationRendition]
        [DiscoveryRendition]
        public void CC06_N3_Ctor2_1OneScalarArgDiff_NEW()
        {
            var res = RunConstructorNEW(nameof(CC06_N3_Ctor2_1OneScalarArgDiff_NEW), docClass =>
            {
                // Blank ctor()
                var constructor = docClass.AddDecoration(MemberFactory.Constructor);
                // Calling the base class
                constructor.AddForwardingArgument(c => c.Output.Dyn5)
                           .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn1);
            });

            var body = Evaluate(res);
            Assert.That(body.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.Count, Is.EqualTo(2));
            Assert.That(body.First().AttributeElements[0].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements[1].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements[0].ChildElements.First.ChildElements.Count, Is.EqualTo(1));
        }


        [Test]
        [NarrationRendition]
        [DiscoveryRendition]
        public void XX06_N3_Ctor2_1OneScalarArgDiff_NEW()
        {
            var res = RunConstructorNEW(nameof(XX06_N3_Ctor2_1OneScalarArgDiff_NEW), docClass =>
            {
                // Blank ctor()
                var constructor = docClass.AddDecoration(MemberFactory.Constructor);
                // Calling the base class
                constructor.AddForwardingArgument(c => c.Output.Dyn5)
                           .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn1);
            });

            var body = Evaluate(res);
            Assert.That(body.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.Count, Is.EqualTo(2));
            Assert.That(body.First().AttributeElements[0].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements[1].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements[0].ChildElements.First.ChildElements.Count, Is.EqualTo(1));
        }


        [Test]
        [NarrationRendition]
        [DiscoveryRendition]
        public void CC07_N3_Ctor2_2OneVectorialArg_NEW()
        {
            var res = RunConstructorNEW(nameof(CC07_N3_Ctor2_2OneVectorialArg_NEW), docClass =>
            {
                // Blank ctor()
                var constructor2 = docClass.AddDecoration(MemberFactory.Constructor);
                // Calling the base class
                constructor2.AddForwardingArgument(c => c.Output.Dyn5)
                            .AddInvokationArgument(SampleUniChart.TypeTemplateB, c => c.Output.Dyn1, c => c.Output.Dyn3);
            });

            var body = Evaluate(res);
            Assert.That(body.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.Count, Is.EqualTo(2));
            Assert.That(body.First().AttributeElements[0].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements[1].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements[0].ChildElements.First.ChildElements.Count, Is.EqualTo(2));
        }


        [Test]
        [NarrationRendition]
        [DiscoveryRendition]
        public void CC08_N3_Ctor2_3TwoArgs_NEW()
        {
            var res = RunConstructorNEW(nameof(CC08_N3_Ctor2_3TwoArgs_NEW), docClass =>
            {
                // Blank ctor()                                                                     // #######################################################
                var constructor2 = docClass.AddDecoration(MemberFactory.Constructor);               // Creation of sticky arguments to match in/fwd arguments? 
                // Calling the base class
                constructor2.AddForwardingArgument(c => c.Output.Dyn5)
                            .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn4)
                            .AddInvokationArgument(SampleUniChart.TypeTemplateB, c => c.Output.Dyn1, c => c.Output.Dyn3);
            });

            var body = Evaluate(res);
            Assert.That(body.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements.Count, Is.EqualTo(2));
            Assert.That(body.First().AttributeElements[0].ChildElements.Count, Is.EqualTo(2));
            Assert.That(body.First().AttributeElements[0].ChildElements[0].ChildElements.Count, Is.EqualTo(1));
            Assert.That(body.First().AttributeElements[0].ChildElements[1].ChildElements.Count, Is.EqualTo(2));
        }


        [Test]
        public void CC09_N3_Ctor1_AmbiguousProbe_Fail_NEW()
        {
            Assert.Warn("Need to fix the Evaluate semantics");
            return;

            var res = RunConstructorNEW(nameof(CC09_N3_Ctor1_AmbiguousProbe_Fail_NEW), docClass =>
            {
                var tp = docClass.GetType().Name;
                // Blank ctor()                                                                     // #######################################################
                var constructor2 = docClass.AddDecoration(MemberFactory.Constructor);               // Creation of sticky arguments to match in/fwd arguments? 
                // Calling the base class
                constructor2
                            .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn4)
                            .AddInvokationArgument(SampleUniChart.TypeTemplateA, c => c.Output.Dyn3)
                            .AddInvokationArgument(SampleUniChart.TypeTemplateB, c => c.Output.Dyn1, c => c.Output.Dyn3);
            }, true);

            var body = Evaluate(res);
            //Assert.That(body.Count, Is.EqualTo(1));
            //Assert.That(body.First().AttributeElements.Count, Is.EqualTo(1));
            //Assert.That(body.First().AttributeElements.First.ChildElements.Count, Is.EqualTo(3));
            //Assert.That(body.First().AttributeElements.First.ChildElements[0].ChildElements.Count, Is.EqualTo(1));
            //Assert.That(body.First().AttributeElements.First.ChildElements[1].ChildElements.Count, Is.EqualTo(1));
            //Assert.That(body.First().AttributeElements.First.ChildElements[2].ChildElements.Count, Is.EqualTo(2));
        }

        #endregion

        #region ToRename

        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        public async Task AA03_N2_AllChapters_Correct_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);

            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    (recipe, repo) =>
                    {
                        // Creating narration
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR1);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));

                        var chapter2 = repo.CreateChapterDefinition(recipe.NVarR2);
                        var chapter2_class1 = chapter2.AddClass(c => c.Output.Dyn1, ND.Str("VarR2"));

                        var chapter3 = repo.CreateChapterDefinition(recipe.NVarR3);
                        var chapter3_class1 = chapter3.AddClass(c => c.Output.Dyn1, ND.Str("VarR3"));

                        var chapter4 = repo.CreateChapterDefinition(recipe.NVarR4);
                        var chapter4_class1 = chapter4.AddClass(c => c.Output.Dyn1, ND.Str("VarR4"));
                        var chapter4_intfc1 = chapter4.AddInterface(c => c.Output.Dyn2, ND.Str("VarR4"));
                    },
                    (recipe, context) =>
                    {
                        // Creating instances/discovery material
                        var ety = context.CreateScope(recipe.NVarR1.CreateSimple(I.R1.R001));
                        var instance = ety.CreateChapter(S.Create("ValueR1"));
                    },
                    renditions: ERenditionRequests.eBlueprint | ERenditionRequests.ePopulation);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(4));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(5));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(1));
        }



        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        public async Task AA04_N2_ChpD1_F1_Correct_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    (recipe, repo) =>
                    {
                        // Creating narration
                        var chapter = repo.CreateChapterDefinition(recipe.NVarR1);
                        var rootClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));
                    },
                    (recipe, context) =>
                    {
                        // Creating instances/discovery material
                        var ety = context.CreateScope(recipe.NVarR1.CreateSimple(I.R1.R001));
                        var instance = ety.CreateChapter(S.Create("ValueR1"));
                    },
                    renditions: ERenditionRequests.eBlueprint | ERenditionRequests.ePopulation);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(1));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(1));
        }



        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        public async Task AA05_N2_ChpD1D2_F1_Correct_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);

            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    (recipe, repo) =>
                    {
                        // Creating narration
                        var chapter = repo.CreateChapterDefinition(recipe.NVarR2);
                        var rootClass = chapter.AddClass(c => c.Output.Dyn1, ND.Str("VarR2"));
                    },
                    (recipe, context) =>
                    {
                        // Creating instances/discovery material
                        var ety = context.CreateScope(recipe.NVarR2.CreateSimple(I.R1.R001));
                        var instance = ety.CreateChapter(S.Create("ValueR1"), S.Create("ValueR2"));
                    },
                    renditions: ERenditionRequests.eBlueprint | ERenditionRequests.ePopulation);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(1));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(1));
        }



        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        public async Task AA06_N2_ChpD1R1_F2_Correct_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                await test.RunAsync(
                    (recipe, repo) =>
                    {
                        // Creating narration
                        var dangling = repo.CreateChapterDefinition(recipe.NVarR3);
                        var cls3 = dangling.AddClass(c => c.Output.Dyn1, ND.Str("VarR3"));

                        var source = repo.CreateChapterDefinition(recipe.NVarR1);
                        var cls1 = source.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));

                        var target = repo.CreateChapterDefinition(recipe.NVarR4);
                        var cls2 = target.AddClass(c => c.Output.Dyn1, ND.Str("VarR4"));

                        // The name of the base class will come from V1
                        var txt = cls2.ConsumeBaseType(OneFieldDynChart.TypeTemplate1, (c) => c.Output.Dyn2);
                    },
                    (recipe, context) =>
                    {
                        // Creating instances/discovery material
                        var vV_D1 = context.CreateScope(recipe.NVarR1.CreateSimple(I.R1.R001));
                        var vV_R1 = context.CreateScope(recipe.NVarR3.CreateSimple(I.R1.R001));
                        var vV_D1R1 = context.CreateScope(recipe.NVarR4.CreateSimple(I.R3.R001));

                        var instSrc = vV_D1.CreateChapter(S.Create("ValueR1"));
                        var instTgt = vV_D1R1.CreateChapter(S.Create("ValueR4"), R.Create(instSrc, "RefR1"));
                        var instRef = vV_R1.CreateChapter(R.Create(instSrc, "RefR1"));
                    },
                    renditions: ERenditionRequests.eBlueprint | ERenditionRequests.ePopulation);
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(3));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(3));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(3));
        }


        [Test(Description = "OneFieldDynChart")]
        [DiscoveryRendition]
        [NarrationRendition]
        public void AA10_N3_CascadedReferencesCorrect_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (recipe, repo) =>
                    {
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR1);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));

                        var chapter2 = repo.CreateChapterDefinition(recipe.NVarR3);
                        var chapter2_class1 = chapter2.AddClass(c => c.Output.Dyn1, ND.Str("VarR3"));

                        var chapterDest = repo.CreateChapterDefinition(recipe.NVarR6);
                        var chapterDest_class1 = chapterDest.AddClass(c => c.Output.Dyn1, ND.Str("VarR6"));
                    },
                    (recipe, context) =>
                    {
                        var etyA = context.CreateScope(recipe.NVarR1.CreateSimple("ScopeRA"));
                        var instanceA = etyA.CreateChapter(S.Create("A_ValueX1"));

                        var etyB = context.CreateScope(recipe.NVarR3.CreateSimple("ScopeRB"));
                        var instanceB = etyB.CreateChapter(R.Create(instanceA));

                        var etyC = context.CreateScope(recipe.NVarR6.CreateSimple("ScopeRC"));
                        var instanceC = etyC.CreateChapter(R.Create(instanceB));
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(3));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(3));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(3));
        }


        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        [DiscoveryRendition]
        public void AA11_N3_MultiVariable3_1_Correct_NEW1()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (recipe, repo) =>
                    {
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR_A);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));

                        var chapter2 = repo.CreateChapterDefinition(recipe.NVarR_B);
                        var chapter2_class1 = chapter2.AddClass(c => c.Output.Dyn1, ND.Str("VarR2"));

                        var chapter3 = repo.CreateChapterDefinition(recipe.NVarR_C);
                        var chapter3_class1 = chapter3.AddClass(c => c.Output.Dyn1, ND.Str("VarR3"));

                        var chapterDest = repo.CreateChapterDefinition(recipe.NVarR_ABC);
                        var chapterDest_class1 = chapterDest.AddClass(c => c.Output.Dyn1, ND.Str("VarR_123"));
                    },
                    (recipe, context) =>
                    {
                        var scA1 = recipe.NVarR_A.CreateSimple("ScopeRA");
                        var scB1 = recipe.NVarR_B.CreateSimple("ScopeRB");
                        var scC1 = recipe.NVarR_C.CreateSimple("ScopeRC");
                        var scA2 = recipe.NVarR_A.CreateSimple("ScopeRA");
                        var scB2 = recipe.NVarR_B.CreateSimple("ScopeRB");
                        var scC2 = recipe.NVarR_C.CreateSimple("ScopeRC");

                        Assert.That(scA1.Subject.Id, Is.Not.EqualTo(scB1.Subject.Id));
                        Assert.That(scA1.Subject.Id, Is.Not.EqualTo(scC1.Subject.Id));
                        Assert.That(scB1.Subject.Id, Is.Not.EqualTo(scC1.Subject.Id));

                        Assert.That(scA1.Subject.Id, Is.EqualTo(scA2.Subject.Id));
                        Assert.That(scB1.Subject.Id, Is.EqualTo(scB2.Subject.Id));
                        Assert.That(scC1.Subject.Id, Is.EqualTo(scC2.Subject.Id));

                        var etyA = context.CreateScope(scA1);
                        var etyB = context.CreateScope(scB1);
                        var etyC = context.CreateScope(scC1);

                        Assert.That(etyA.Subject.Id, Is.EqualTo(scA1.Subject.Id));
                        Assert.That(etyB.Subject.Id, Is.EqualTo(scB1.Subject.Id));
                        Assert.That(etyC.Subject.Id, Is.EqualTo(scC1.Subject.Id));

                        var instanceA = etyA.CreateChapter(S.Create("A_ValueX1"), S.Create("A_ValueX2"));
                        var instanceB = etyB.CreateChapter(S.Create("B_ValueX1"), S.Create("B_ValueX2"));
                        var instanceC = etyC.CreateChapter(S.Create("C_ValueX1"), S.Create("C_ValueX2"));

                        var etyABC = context.CreateScope(recipe.NVarR_ABC.CreateSimple("ScopeRABC"));
                        var instanceABC = etyABC.CreateChapter(R.Create(instanceA), R.Create(instanceB), R.Create(instanceC));
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(4));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(4));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(4));
        }


        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        [DiscoveryRendition]
        //public void AA_VAR1()
        public void AA11_N3_MultiVariable3_1_Correct_NEW2()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (setup) =>
                    {
                        // Instead of using zillions of separate classes to point to different targets, we just 
                        // setup the pointing to whereever - here instead
                        var useThisCreation = setup
                                  .For(rec => rec.NVarR1).XVarPipeline_In1Out2_ClsItf()
                                                         .ConfigSink(r => r.X1).Simple()
                                  .For(rec => rec.NVarR3).XVarPipeline_In1Out2_ClsItf()
                                                         .ConfigSink(r => r.X1).WithLinkTo(s => s.NVarR1, t => t.Output.Dyn1)
                                  .For(rec => rec.NVarR5).XVarPipeline_In1Out2_ClsItf()
                                                         .ConfigSink(r => r.X1).Simple()
                                  .For(rec => rec.NVarR6).XVarPipeline_In1Out2_ClsItf()
                                                         .ConfigSink(r => r.X1).WithLinkTo(s => s.NVarR3, t => t.Output.Dyn1)
                                  .For(rec => rec.NVarR2).XVarPipeline_In2Out1()
                                                         .ConfigSink(r => r.X1).Simple()
                                                         .ConfigSink(r => r.X2).Simple()
                                  .For(rec => rec.NVarR_AB).XVarPipeline_In2Out1()
                                                           .ConfigSink(r => r.X1).WithLinkTo(s => s.NVarR_A, t => t.Output.Dyn2)
                                                           .ConfigSink(r => r.X2).WithLinkTo(s => s.NVarR_B, t => t.Output.Dyn2)
                                  .For(rec => rec.NVarR4).XVarPipeline_In2Out2()
                                                         .ConfigSink(r => r.X1).Simple()
                                                         .ConfigSink(r => r.X2).WithLinkTo(s => s.NVarR1, t => t.Output.Dyn1)
                                  .For(rec => rec.NVarR_A).XVarPipeline_In2Out2()
                                                          .ConfigSink(r => r.X1).Simple()
                                                          .ConfigSink(r => r.X2).Simple()
                                  .For(rec => rec.NVarR_B).XVarPipeline_In2Out2()
                                                          .ConfigSink(r => r.X1).Simple()
                                                          .ConfigSink(r => r.X2).Simple()
                                  .For(rec => rec.NVarR_C).XVarPipeline_In2Out2()
                                                          .ConfigSink(r => r.X1).Simple()
                                                          .ConfigSink(r => r.X2).Simple()
                                  .For(rec => rec.NVarR_ABC).XVarPipeline_In3Out1()
                                                            .ConfigSink(r => r.X1).WithLinkTo(s => s.NVarR_A, t => t.Output.Dyn1)
                                                            .ConfigSink(r => r.X2).WithLinkTo(s => s.NVarR_B, t => t.Output.Dyn1)
                                                            .ConfigSink(r => r.X3).WithLinkTo(s => s.NVarR_C, t => t.Output.Dyn1)
                                  .Publish();

                        return useThisCreation;
                    },
                    (recipe, repo) =>
                    {
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR_A);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));

                        var chapter2 = repo.CreateChapterDefinition(recipe.NVarR_B);
                        var chapter2_class1 = chapter2.AddClass(c => c.Output.Dyn1, ND.Str("VarR2"));

                        var chapter3 = repo.CreateChapterDefinition(recipe.NVarR_C);
                        var chapter3_class1 = chapter3.AddClass(c => c.Output.Dyn1, ND.Str("VarR3"));

                        var chapterDest = repo.CreateChapterDefinition(recipe.NVarR_ABC);
                        var chapterDest_class1 = chapterDest.AddClass(c => c.Output.Dyn1, ND.Str("VarR_123"));
                    },
                    (recipe, context) =>
                    {
                        var scA1 = recipe.NVarR_A.CreateSimple("ScopeRA");
                        var scB1 = recipe.NVarR_B.CreateSimple("ScopeRB");
                        var scC1 = recipe.NVarR_C.CreateSimple("ScopeRC");
                        var scA2 = recipe.NVarR_A.CreateSimple("ScopeRA");
                        var scB2 = recipe.NVarR_B.CreateSimple("ScopeRB");
                        var scC2 = recipe.NVarR_C.CreateSimple("ScopeRC");

                        Assert.That(scA1.Subject.Id, Is.Not.EqualTo(scB1.Subject.Id));
                        Assert.That(scA1.Subject.Id, Is.Not.EqualTo(scC1.Subject.Id));
                        Assert.That(scB1.Subject.Id, Is.Not.EqualTo(scC1.Subject.Id));

                        Assert.That(scA1.Subject.Id, Is.EqualTo(scA2.Subject.Id));
                        Assert.That(scB1.Subject.Id, Is.EqualTo(scB2.Subject.Id));
                        Assert.That(scC1.Subject.Id, Is.EqualTo(scC2.Subject.Id));

                        var etyA = context.CreateScope(scA1);
                        var etyB = context.CreateScope(scB1);
                        var etyC = context.CreateScope(scC1);

                        Assert.That(etyA.Subject.Id, Is.EqualTo(scA1.Subject.Id));
                        Assert.That(etyB.Subject.Id, Is.EqualTo(scB1.Subject.Id));
                        Assert.That(etyC.Subject.Id, Is.EqualTo(scC1.Subject.Id));

                        var instanceA = etyA.CreateChapter(S.Create("A_ValueX1"), S.Create("A_ValueX2"));
                        var instanceB = etyB.CreateChapter(S.Create("B_ValueX1"), S.Create("B_ValueX2"));
                        var instanceC = etyC.CreateChapter(S.Create("C_ValueX1"), S.Create("C_ValueX2"));

                        var etyABC = context.CreateScope(recipe.NVarR_ABC.CreateSimple("ScopeRABC"));
                        var instanceABC = etyABC.CreateChapter(R.Create(instanceA), R.Create(instanceB), R.Create(instanceC));
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(4));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(4));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(4));
        }


        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        [DiscoveryRendition]
        //public void A__000()
        public void AA12_N3_MultiVariable2_1_Correct_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (recipe, repo) =>
                    {
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR_A);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));

                        var chapter2 = repo.CreateChapterDefinition(recipe.NVarR_B);
                        var chapter2_class1 = chapter2.AddClass(c => c.Output.Dyn1, ND.Str("VarR2"));

                        var chapterDest = repo.CreateChapterDefinition(recipe.NVarR_AB);
                        var chapterDest_class1 = chapterDest.AddClass(c => c.Output.Dyn1, ND.Str("VarR_12"));
                    },
                    (recipe, context) =>
                    {
                        var etyA = context.CreateScope(recipe.NVarR_A.CreateSimple("ScopeRA"));
                        var instanceA = etyA.CreateChapter(S.Create("A_ValueX1"), S.Create("A_ValueX2"));

                        var etyB = context.CreateScope(recipe.NVarR_B.CreateSimple("ScopeRB"));
                        var instanceB = etyB.CreateChapter(S.Create("B_ValueX1"), S.Create("B_ValueX2"));

                        var etyAB = context.CreateScope(recipe.NVarR_AB.CreateSimple("ScopeRAB"));
                        var instanceAB = etyAB.CreateChapter(R.Create(instanceA), R.Create(instanceB));
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(3));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(3));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(3));
        }


        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        [DiscoveryRendition]
        public void AA13_N3_MinimalChapter_Correct_NEW()
        {
            //if (newStyle)
            //{
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);

            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (recipe, repo) =>
                    {
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR1);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));
                    },
                    (recipe, context) =>
                    {
                        var ety = context.CreateScope(recipe.NVarR1.CreateSimple("ScopeR1"));
                        var instance = ety.CreateChapter(S.Create("ValueR1"));
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(1));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(1));
        }



        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        [DiscoveryRendition]
        public void AA14_N3_ChaptersRepeated_Correct_NEW()
        {
            var MAXCHP = 5;

            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (recipe, repo) =>
                    {
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR1);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));
                    },
                    (recipe, context) =>
                    {
                        for (int i = 0; i < MAXCHP; i++)
                        {
                            var ety = context.CreateScope(recipe.NVarR1.CreateSimple($"ScopeR{i + 1}"));
                            var instance = ety.CreateChapter(S.Create($"ValueR{i + 1}"));
                        }
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(1));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(MAXCHP));
        }



        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        [DiscoveryRendition]
        //public void AA15_N3_RealBusinessObjects_Correct_NEW()
        public void ZZZZZ1()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (recipe, repo) =>
                    {
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR1);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("Entity"));

                        chapter1_class1.AddDecoration(MemberFactory.Field(SampleUniChart.TypeTemplate1), c => c.Output.Dyn2, c => c.Output.Dyn2);        // Same for Cls
                    },
                    (recipe, context) =>
                    {
                        var etyA = context.CreateScope(recipe.NVarR1.CreateSimple("UserSpecificationScope"));
                        var instanceA = etyA.CreateChapter(S.Create("UserSpecification"));

                        var etyB = context.CreateScope(recipe.NVarR1.CreateSimple("UserGroupSpecificationScope"));
                        var instanceB = etyB.CreateChapter(S.Create("UserGroupSpecification"));
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(1));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(2));
        }


        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        [DiscoveryRendition]
        public void AA18_N3_OneRefChapter_Correct_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (recipe, repo) =>
                    {
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR1);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));

                        var chapter2 = repo.CreateChapterDefinition(recipe.NVarR3);
                        var chapter2_class1 = chapter2.AddClass(c => c.Output.Dyn1, ND.Str("VarR3"));
                    },
                    (recipe, context) =>
                    {
                        var ety = context.CreateScope(recipe.NVarR1.CreateSimple(I.R1.R001));
                        var instance1 = ety.CreateChapter(S.Create("ValueR1"));

                        var tgt = context.CreateScope(recipe.NVarR3.CreateSimple(I.R1.R001));
                        var instance2 = tgt.CreateChapter(R.Create(instance1));
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(2));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(2));
            Assert.That(output.VariableReport.InputVariables.Count, Is.EqualTo(2));
        }


        [Test(Description = "OneFieldDynChart")]
        [NarrationRendition]
        public void AA19_N3_NoChapters_EmptyDiscovery_NEW()
        {
            var setup = NarrationTestCaseSetup.Create<OneFieldDynChart>();
            var output = setup.CreateContainer(this);

            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                test.Run(
                    (recipe, repo) =>
                    {
                        var chapter1 = repo.CreateChapterDefinition(recipe.NVarR1);
                        var chapter1_class1 = chapter1.AddClass(c => c.Output.Dyn1, ND.Str("VarR1"));
                    });
            }

            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.ChapterDefinition), Is.EqualTo(1));
            Assert.That(output.InNarration(NarrationColumns.SectionType, SectionTypes.AbstractionDefinition), Is.EqualTo(1));
            Assert.That(output.InDiscInput(DiscoveryColumns.NarrationTag, NarrationTags.InstanceTypes.Chapter), Is.EqualTo(0));
            Assert.That(output.VariableReport.InputVariables.Count, Is.EqualTo(0));
        }

        #endregion

        [Test]
        public void UU_Mismatch_P1X1_Error()
        {
            var ex = Assert.Throws<DynamicRecipeException>(() =>
            {
                var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
                var output = setup.CreateContainer(this);
                using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
                {
                    var emitData = SetupPublishing(test);
                    test.Run(
                        (composer) =>
                        {
                            var setup = composer.For(chart => chart.NA1).XVarPipeline_In1Out2_ClsItf()
                                                                        .ConfigSink(pip => pip.P1).Simple()
                                                .For(chart => chart.NB1).XVarPipeline_In1Out2_ClsItf()
                                                                        .ConfigSink(pip => pip.X1).Simple()
                                                .Publish();
                            return setup;
                        });
                }
            });
            Assert.That(ex.ErrCode, Is.EqualTo(NarrationErrors.ErrCodes.eIllegalParameterMode));
        }


        [Test]
        public void UU_Mismatch_MissingTestSteps()
        {
            var setup = NarrationTestCaseSetup.Create<SampleUniChart>(DataChest, C.Instance.Template.None);
            var output = setup.CreateContainer(this);
            using (var test = setup.CreateTest(output, TestAdapterIdentity.Create(TestContext.CurrentContext.Test)))
            {
                var emitData = SetupPublishing(test);
                test.Run(
                    (composer) =>
                    {
                        var setup = composer.For(chart => chart.NA1).XVarPipeline_In1Out2_ClsItf()
                                                                    .ConfigSink(pip => pip.X1).Simple()
                                            .For(chart => chart.NB1).XVarPipeline_In1Out2_ClsItf()
                                                                    .ConfigSink(pip => pip.X1).Simple()
                                            .Publish();
                        return setup;
                    });
            }
        }
      
    }
}
