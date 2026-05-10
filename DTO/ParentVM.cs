namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class ParentVM
    {
        public ProfileVM ProfileInfo { get; set; } = new ProfileVM();
        public ChangePasswordVM ChangePassword { get; set; } = new ChangePasswordVM();
    }
}
