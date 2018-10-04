
public enum ItemSlot : byte
{
    PRIMARY, // Can only be stored in the PRIMARY slot (diagonally on the back)
    SECODNARY, // Can only be stored in the SECONDARY slot (vertical on the back)
    BACKUP, // Can only be stored in the BACKUP slot (on hip or waist)
    STORED, // Can be stored on the character, but normally invisible. Cannot be stored in primary, secondary or backup
    NO_SLOT // Can never be stored on the character (and therefore cannot have quick-access keys)
}
