﻿// <copyright file="UpdateLevelPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateLevelPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateLevelPlugIn", "The default implementation of the IUpdateLevelPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("1ff3709e-d99b-4c00-b926-efce281b3997")]
    public class UpdateLevelPlugIn : IUpdateLevelPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLevelPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateLevelPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateLevel()
        {
            var selectedCharacter = this.player.SelectedCharacter;
            var charStats = this.player.Attributes;
            if (selectedCharacter is null || charStats is null)
            {
                return;
            }

            this.player.Connection?.SendCharacterLevelUpdate(
                (ushort)charStats[Stats.Level],
                (ushort)selectedCharacter.LevelUpPoints,
                (ushort)charStats[Stats.MaximumHealth],
                (ushort)charStats[Stats.MaximumMana],
                (ushort)charStats[Stats.MaximumShield],
                (ushort)charStats[Stats.MaximumAbility],
                (ushort)selectedCharacter.UsedFruitPoints,
                selectedCharacter.GetMaximumFruitPoints(),
                (ushort)selectedCharacter.UsedNegFruitPoints,
                selectedCharacter.GetMaximumFruitPoints());

            this.player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage($"Congratulations, you are Level {charStats[Stats.Level]} now.", MessageType.BlueNormal);
        }
    }
}