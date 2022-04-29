namespace WowPacketParser.Enums
{
    public enum ArenaError : uint
    {
        NoTeam              = 0, // ERR_ARENA_NO_TEAM_II
        ExpiredCAIS         = 1, // ERR_ARENA_EXPIRED_CAIS
        CantUseBattleground = 2  // ERR_LFG_CANT_USE_BATTLEGROUND
    }

    public enum ArenaTeamCommandErrors243
    {
        None                        = 0x00,
        Internal                    = 0x01,
        AlreadyInArenaTeam          = 0x02,
        AlreadyInArenaTeamS         = 0x03,
        InvitedToArenaTeam          = 0x04,
        AlreadyInvitedToArenaTeamS  = 0x05,
        NameInvalid                 = 0x06,
        NameExistsS                 = 0x07,
        LeaderLeaveS                = 0x08,
        Permissions                 = 0x08,
        PlayerNotInTeam             = 0x09,
        PlayerNotInTeamSS           = 0x0A,
        PlayerNotFoundS             = 0x0B,
        NotALlied                   = 0x0C,
        IgnoringYouS                = 0x13,
        TargetTooLowS               = 0x15,
        TooManyMembersS             = 0x16,
    }

    public enum ArenaTeamCommandErrors254
    {
        None                        = 0x00,
        Internal                    = 0x01,
        AlreadyInArenaTeam          = 0x02,
        AlreadyInArenaTeamS         = 0x03,
        InvitedToArenaTeam          = 0x04,
        AlreadyInvitedToArenaTeamS  = 0x05,
        NameInvalid                 = 0x06,
        NameExistsS                 = 0x07,
        LeaderLeaveS                = 0x08,
        Permissions                 = 0x08,
        PlayerNotInTeam             = 0x09,
        PlayerNotInTeamSS           = 0x0A,
        PlayerNotFoundS             = 0x0B,
        NotALlied                   = 0x0C,
        IgnoringYouS                = 0x13,
        Internal2                   = 0x14,
        TargetTooLowS               = 0x15,
        TargetTooHighS              = 0x16,
        TooManyMembersS             = 0x17,
        NotFound                    = 0x1B,
        Locked                      = 0x1E,
        TooManyCreate               = 0x21,
        Disqualified                = 0x2A,
    }
}
