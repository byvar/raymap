using BinarySerializer;

namespace BinarySerializer.Ubisoft.CPA {
    public class EngineVersionTree : VersionTree<EngineVersion> {
        public static EngineVersionTree Create(CPA_Settings settings) {
            EngineVersionTree tree = new EngineVersionTree() {
                Root = new Node(EngineVersion.CPA).SetChildren(
                    new Node(EngineVersion.CPA_1).SetChildren(
                        new Node(EngineVersion.TonicTroubleSE).SetChildren(
                            new Node(EngineVersion.TonicTrouble).SetChildren(
                                new Node(EngineVersion.CPA_2).SetChildren(
                                    new Node(EngineVersion.Rayman2Demo).SetChildren(
                                        new Node(EngineVersion.RedPlanet),
                                        new Node(EngineVersion.Rayman2).SetChildren(
                                            new Node(EngineVersion.Rayman2Revolution),
                                            new Node(EngineVersion.DonaldDuckQuackAttack),
											new Node(EngineVersion.Rayman4DS).SetChildren(
												new Node(EngineVersion.Rayman4DS_20060525).SetChildren(
													new Node(EngineVersion.RaymanRavingRabbids).SetChildren(
														new Node(EngineVersion.Rayman2_3D)
													)
												)
											),
                                            new Node(EngineVersion.CPA_3).SetChildren(
                                                new Node(EngineVersion.Dinosaur).SetChildren(
                                                    new Node(EngineVersion.RaymanM).SetChildren(
                                                        new Node(EngineVersion.RaymanArena),
                                                        new Node(EngineVersion.Rayman3).SetChildren(
                                                            new Node(EngineVersion.DonaldDuckPK)
                                                        )
                                                    ),
                                                    new Node(EngineVersion.LargoWinch)
                                                )
                                            )
                                        )
                                    )
                                ),
                                new Node(EngineVersion.CPA_PS1).SetChildren(
                                    new Node(EngineVersion.Rayman2_PS1).SetChildren(
                                        new Node(EngineVersion.VIP_PS1),
                                        new Node(EngineVersion.RaymanRush_PS1),
                                        new Node(EngineVersion.DonaldDuckQuackAttack_PS1),
                                        new Node(EngineVersion.JungleBook_PS1)
                                    )
                                )
                            )
                        )
                    ),
                    new Node(EngineVersion.CPA_Montreal).SetChildren(
                        new Node(EngineVersion.PlaymobilLaura).SetChildren(
							new Node(EngineVersion.PlaymobilHype).SetChildren(
								new Node(EngineVersion.PlaymobilAlex)
							)
						)
					)
                )
            };
            tree.Init();
            tree.Current = tree.FindVersion(settings.EngineVersion);

            return tree;
        }
    }
}