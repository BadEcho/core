using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("19d8e431-bf6c-46bd-ac7b-4195fa909ea4")]

[assembly: SuppressMessage("Security", 
                           "CA5394:Do not use insecure randomness", 
                           Scope = "member", 
                           Target = "~M:BadEcho.Game.ParticleSystem.GenerateParticle~BadEcho.Game.Particle",
                           Justification = "Random is being used for visual effects purposes, not security.")]
