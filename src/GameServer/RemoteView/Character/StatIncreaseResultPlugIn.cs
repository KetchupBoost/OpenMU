﻿// <copyright file="StatIncreaseResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IStatIncreaseResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("StatIncreaseResultPlugIn", "The default implementation of the IStatIncreaseResultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("ce603b3c-cf25-426f-9cb9-5cc367843de8")]
    public class StatIncreaseResultPlugIn : IStatIncreaseResultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatIncreaseResultPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public StatIncreaseResultPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void StatIncreaseResult(AttributeDefinition attribute, bool success)
        {
            byte successAndStatType = (byte)attribute.GetStatType();
            if (success)
            {
                successAndStatType += 1 << 4;
            }

            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x0C))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x06;
                packet[4] = successAndStatType;
                if (success)
                {
                    if (attribute == Stats.BaseEnergy)
                    {
                        packet.Slice(6).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumMana]);
                    }
                    else if (attribute == Stats.BaseVitality)
                    {
                        packet.Slice(6).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumHealth]);
                    }
                    else
                    {
                        // no updated value required at index 6
                    }

                    packet.Slice(8).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumShield]);
                    packet.Slice(10).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumAbility]);
                }

                writer.Commit();
            }
        }
    }
}