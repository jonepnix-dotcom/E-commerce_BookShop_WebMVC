using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TheLight_JoneBookShop_WebMVC.Models;

namespace TheLight_JoneBookShop_WebMVC.Data;

public partial class DbjonebookshopContext : DbContext
{
    public DbjonebookshopContext()
    {
    }

    public DbjonebookshopContext(DbContextOptions<DbjonebookshopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<Bookorder> Bookorders { get; set; }

    public virtual DbSet<Booktype> Booktypes { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<Evaluate> Evaluates { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Loginhistory> Loginhistories { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Shopcart> Shopcarts { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Tax> Taxes { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<Warehousetransaction> Warehousetransactions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Idauthor).HasName("PK__author__FE2889A47914AFB8");

            entity.ToTable("author");

            entity.Property(e => e.Idauthor).HasColumnName("IDAuthor");
            entity.Property(e => e.AuthorName).HasMaxLength(255);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Idbook).HasName("PK__book__2339855F29E15E95");

            entity.ToTable("book");

            entity.Property(e => e.Idbook).HasColumnName("IDBook");
            entity.Property(e => e.BookName).HasMaxLength(255);
            entity.Property(e => e.IdbookType).HasColumnName("IDBookType");
            entity.Property(e => e.Iddiscount).HasColumnName("IDDiscount");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdbookTypeNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.IdbookType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__book__IDBookType__5535A963");

            entity.HasOne(d => d.IddiscountNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.Iddiscount)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_book_discount");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.ToTable("book_author");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Idauthor).HasColumnName("IDAuthor");
            entity.Property(e => e.Idbook).HasColumnName("IDbook");

            entity.HasOne(d => d.IdauthorNavigation).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.Idauthor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_book_author_author");

            entity.HasOne(d => d.IdbookNavigation).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.Idbook)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_book_author_book1");
        });

        modelBuilder.Entity<Bookorder>(entity =>
        {
            entity.HasKey(e => e.IdbookOrder).HasName("PK__bookorde__681A27437397F9D9");

            entity.ToTable("bookorder");

            entity.Property(e => e.IdbookOrder).HasColumnName("IDBookOrder");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.Iddiscount).HasColumnName("IDDiscount");
            entity.Property(e => e.Idpayment).HasColumnName("IDPayment");
            entity.Property(e => e.Idtax).HasColumnName("IDTax");
            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.Idvoucher).HasColumnName("IDVoucher");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IddiscountNavigation).WithMany(p => p.Bookorders)
                .HasForeignKey(d => d.Iddiscount)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bookorder_discount");

            entity.HasOne(d => d.IdpaymentNavigation).WithMany(p => p.Bookorders)
                .HasForeignKey(d => d.Idpayment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bookorder_payment");

            entity.HasOne(d => d.IdtaxNavigation).WithMany(p => p.Bookorders)
                .HasForeignKey(d => d.Idtax)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bookorder_tax");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Bookorders)
                .HasForeignKey(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookorder__IDUse__5629CD9C");

            entity.HasOne(d => d.IdvoucherNavigation).WithMany(p => p.Bookorders)
                .HasForeignKey(d => d.Idvoucher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bookorder_Vouchers");
        });

        modelBuilder.Entity<Booktype>(entity =>
        {
            entity.HasKey(e => e.IdbookType).HasName("PK__booktype__8BF9CD8056B9D8CE");

            entity.ToTable("booktype");

            entity.Property(e => e.IdbookType).HasColumnName("IDBookType");
            entity.Property(e => e.BookTypeName).HasMaxLength(255);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Iduser).HasName("PK__client__EAE6D9DF96B28E78");

            entity.ToTable("client", tb => tb.HasTrigger("trg_SetAuthorizeDefault"));

            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phones).HasMaxLength(20);
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(255);
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Iddiscount).HasName("PK__discount__D196D8720C003139");

            entity.ToTable("discount");

            entity.Property(e => e.Iddiscount).HasColumnName("IDDiscount");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailVer__3214EC07E85AAABC");

            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.EmailVerifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailVerifications_Users");
        });

        modelBuilder.Entity<Evaluate>(entity =>
        {
            entity.HasKey(e => e.Idevaluate).HasName("PK__evaluate__7C7E9FAEEE394EEF");

            entity.ToTable("evaluate");

            entity.Property(e => e.Idevaluate).HasColumnName("IDEvaluate");
            entity.Property(e => e.Content).HasMaxLength(1024);
            entity.Property(e => e.EvaluationDate).HasColumnType("datetime");
            entity.Property(e => e.Idbook).HasColumnName("IDBook");
            entity.Property(e => e.Iduser).HasColumnName("IDUser");

            entity.HasOne(d => d.IdbookNavigation).WithMany(p => p.Evaluates)
                .HasForeignKey(d => d.Idbook)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__evaluate__IDBook__59063A47");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Evaluates)
                .HasForeignKey(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__evaluate__IDUser__59FA5E80");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Idimage).HasName("PK__image__365310E88737E86A");

            entity.ToTable("image");

            entity.Property(e => e.Idimage).HasColumnName("IDImage");
            entity.Property(e => e.Idbook).HasColumnName("IDBook");
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.IdbookNavigation).WithMany(p => p.Images)
                .HasForeignKey(d => d.Idbook)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__image__IDBook__5AEE82B9");
        });

        modelBuilder.Entity<Loginhistory>(entity =>
        {
            entity.HasKey(e => e.IdloginHistory).HasName("PK__loginhis__36112FF4CE208522");

            entity.ToTable("loginhistory");

            entity.Property(e => e.IdloginHistory).HasColumnName("IDLoginHistory");
            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.LoginTime).HasColumnType("datetime");
            entity.Property(e => e.LogoutTime).HasColumnType("datetime");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Loginhistories)
                .HasForeignKey(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__loginhist__IDUse__5BE2A6F2");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => e.IdorderDetails).HasName("PK__orderdet__66008E7395CA2DAC");

            entity.ToTable("orderdetails", tb =>
                {
                    tb.HasTrigger("trg_SetPriceFromBook");
                    tb.HasTrigger("trg_UpdateTotalPrice");
                });

            entity.Property(e => e.IdorderDetails).HasColumnName("IDOrderDetails");
            entity.Property(e => e.Idbook).HasColumnName("IDBook");
            entity.Property(e => e.IdbookOrder).HasColumnName("IDBookOrder");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdbookNavigation).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.Idbook)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__orderdeta__IDBoo__5DCAEF64");

            entity.HasOne(d => d.IdbookOrderNavigation).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.IdbookOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__orderdeta__IDBoo__5CD6CB2B");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Idpayment);

            entity.ToTable("payment");

            entity.Property(e => e.Idpayment).HasColumnName("IDPayment");
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<Shopcart>(entity =>
        {
            entity.HasKey(e => e.IdshopCart).HasName("PK__shopcart__236C459A95E3F7FD");

            entity.ToTable("shopcart");

            entity.Property(e => e.IdshopCart).HasColumnName("IDShopCart");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.Idbook).HasColumnName("IDBook");
            entity.Property(e => e.Idsession)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("IDSession");
            entity.Property(e => e.Iduser).HasColumnName("IDUser");

            entity.HasOne(d => d.IdbookNavigation).WithMany(p => p.Shopcarts)
                .HasForeignKey(d => d.Idbook)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__shopcart__IDBook__5EBF139D");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Shopcarts)
                .HasForeignKey(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__shopcart__IDUser__5FB337D6");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Idsupplier).HasName("PK__supplier__0851A1EDDE4B930B");

            entity.ToTable("supplier");

            entity.Property(e => e.Idsupplier).HasColumnName("IDSupplier");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Phones).HasMaxLength(20);
            entity.Property(e => e.SupplierName).HasMaxLength(255);
        });

        modelBuilder.Entity<Tax>(entity =>
        {
            entity.HasKey(e => e.Idtax);

            entity.ToTable("tax");

            entity.Property(e => e.Idtax).HasColumnName("IDTax");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.Tax1).HasColumnName("Tax");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vouchers__3214EC07572C7560");

            entity.HasIndex(e => e.Code, "UQ__Vouchers__A25C5AA70DFDD40D").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.MaxDiscount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MinOrderValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UsedCount).HasDefaultValue(0);
        });

        modelBuilder.Entity<Warehousetransaction>(entity =>
        {
            entity.HasKey(e => e.IdwarehouseTransaction).HasName("PK__warehous__7DC8671EEC7EC65F");

            entity.ToTable("warehousetransaction");

            entity.Property(e => e.IdwarehouseTransaction).HasColumnName("IDWarehouseTransaction");
            entity.Property(e => e.Idbook).HasColumnName("IDBook");
            entity.Property(e => e.Idsupplier).HasColumnName("IDSupplier");
            entity.Property(e => e.TransactionType).HasMaxLength(50);

            entity.HasOne(d => d.IdbookNavigation).WithMany(p => p.Warehousetransactions)
                .HasForeignKey(d => d.Idbook)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__warehouse__IDBoo__60A75C0F");

            entity.HasOne(d => d.IdsupplierNavigation).WithMany(p => p.Warehousetransactions)
                .HasForeignKey(d => d.Idsupplier)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__warehouse__IDSup__619B8048");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
