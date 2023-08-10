namespace BinarySerializer.Ubisoft.CPA
{
	public class EngineVersionTree : VersionTree<EngineVersion> 
    {
	    public EngineVersionTree(Node root) : base(root) { }

		public static EngineVersionTree Create(CPA_Settings settings) {
			EngineVersionTree tree = new(
				root: new Node(EngineVersion.CPA) {
					new Node(EngineVersion.CPA_1) {
						new Node(EngineVersion.TonicTroubleSE) {
							new Node(EngineVersion.TonicTrouble) {
								new Node(EngineVersion.CPA_2) {
									new Node(EngineVersion.Rayman2Demo) {
										new Node(EngineVersion.RedPlanet),
										new Node(EngineVersion.Rayman2) {
											new Node(EngineVersion.Rayman2Revolution) {
												new Node(EngineVersion.LargoWinch)
											},
											new Node(EngineVersion.DonaldDuckQuackAttackDemo) {
												new Node(EngineVersion.DonaldDuckQuackAttack)
											},
											new Node(EngineVersion.Rayman4DS) {
												new Node(EngineVersion.Rayman4DS_20060525) {
													new Node(EngineVersion.RaymanRavingRabbids) {
														new Node(EngineVersion.Rayman2_3D)
													}
												}
											},
                                            new Node(EngineVersion.CPA_3) {
                                                new Node(EngineVersion.Dinosaur) {
                                                    new Node(EngineVersion.RaymanM) {
                                                        new Node(EngineVersion.RaymanArena) {
															new Node(EngineVersion.Rayman3) {
																new Node(EngineVersion.DonaldDuckPK)
															}
														}
                                                    }
                                                }
											},
											new Node(EngineVersion.CPA_Montreal2)
                                        }
                                    }
								},
                                new Node(EngineVersion.CPA_PS1) {
                                    new Node(EngineVersion.Rayman2_PS1) {
                                        new Node(EngineVersion.VIP_PS1),
                                        new Node(EngineVersion.RaymanRush_PS1),
                                        new Node(EngineVersion.DonaldDuckQuackAttack_PS1),
                                        new Node(EngineVersion.JungleBook_PS1)
                                    }
                                }
                            }
                        }
					},
                    new Node(EngineVersion.CPA_Montreal) {
                        new Node(EngineVersion.PlaymobilLaura) {
							new Node(EngineVersion.PlaymobilHype) {
								new Node(EngineVersion.PlaymobilAlex)
							}
						}
					}
				}
			);
            tree.Init();
            tree.Current = tree.FindVersion(settings.EngineVersion);

            return tree;
        }
    }
}