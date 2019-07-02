using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CharceApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       public DbSet<PersonalAccount> personalaccounts { get; set; }
       public DbSet<BusinessAccount> businessaccounts { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<FilePath> FilePaths { get; set; }
        public DbSet<ProfilePic> profilepics { get; set; }
        public DbSet<File_Business> File_Businesses { get; set; }
        public DbSet<FilePath_Business> FilePath_Businesses { get; set; }
        public DbSet<ProfilePic_Business> profilepic_businesses { get; set; }
        public DbSet<File_Product> File_Products { get; set; }
        public DbSet<FilePath_Product> FilePath_Products { get; set; }
        public DbSet<ProfilePic_Product> profilepic_products { get; set; }
        public DbSet<ActiveProfile> activeprofiles { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<Conversation> conversations { get; set; }
        public DbSet<Message> messages { get; set; }
        public DbSet<NewMessageNotification> newmessagenotifications { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<ListOrder> listorders { get; set; }
        public DbSet<ProductListOrder> productlistorders { get; set; }
        public DbSet<ChatScreen> chatscreens { get; set; }

    }
}