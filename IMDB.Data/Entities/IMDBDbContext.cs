using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Data.Entities;

public partial class IMDBDbContext : DbContext
{
    public IMDBDbContext()
    {
    }

    public IMDBDbContext(DbContextOptions<IMDBDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<ActorsRating> ActorsRatings { get; set; }

    public virtual DbSet<Title> Titles { get; set; }

    public virtual DbSet<TitleAka> TitleAkas { get; set; }

    public virtual DbSet<TitleEpisode> TitleEpisodes { get; set; }

    public virtual DbSet<TitlePrincipal> TitlePrincipals { get; set; }

    public virtual DbSet<TitleRating> TitleRatings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivityHistory> UserActivityHistories { get; set; }

    public virtual DbSet<UserBookmark> UserBookmarks { get; set; }

    public virtual DbSet<UserNote> UserNotes { get; set; }

    public virtual DbSet<UserRatingHistory> UserRatingHistories { get; set; }

    public virtual DbSet<WordIndex> WordIndices { get; set; }

    public virtual DbSet<WordIndexTitle> WordIndexTitles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(e => e.NameId).HasName("actors_pkey");

            entity.ToTable("actors");

            entity.Property(e => e.NameId)
                .HasMaxLength(50)
                .HasColumnName("name_id");
            entity.Property(e => e.BirthYear).HasColumnName("birth_year");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeathYear).HasColumnName("death_year");
            entity.Property(e => e.PrimaryName).HasColumnName("primary_name");
            entity.Property(e => e.PrimaryProfession).HasColumnName("primary_profession");
        });

        modelBuilder.Entity<ActorsRating>(entity =>
        {
            entity.HasKey(e => e.NameId).HasName("actors_ratings_pkey");

            entity.ToTable("actors_ratings");

            entity.Property(e => e.NameId)
                .HasMaxLength(50)
                .HasColumnName("name_id");
            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_updated_at");
            entity.Property(e => e.TotalVotes).HasColumnName("total_votes");
            entity.Property(e => e.WeightedRating).HasColumnName("weighted_rating");
        });

        modelBuilder.Entity<Title>(entity =>
        {
            entity.HasKey(e => e.TitleId).HasName("titles_pkey");

            entity.ToTable("titles");

            entity.Property(e => e.TitleId)
                .HasMaxLength(50)
                .HasColumnName("title_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EndYear).HasColumnName("end_year");
            entity.Property(e => e.Genres).HasColumnName("genres");
            entity.Property(e => e.OriginalTitle).HasColumnName("original_title");
            entity.Property(e => e.Plot).HasColumnName("plot");
            entity.Property(e => e.PrimaryTitle).HasColumnName("primary_title");
            entity.Property(e => e.RuntimeMinutes).HasColumnName("runtime_minutes");
            entity.Property(e => e.StartYear).HasColumnName("start_year");
            entity.Property(e => e.TitleType)
                .HasMaxLength(50)
                .HasColumnName("title_type");
        });

        modelBuilder.Entity<TitleAka>(entity =>
        {
            entity.HasKey(e => e.AkaId).HasName("title_akas_pkey");

            entity.ToTable("title_akas");

            entity.HasIndex(e => new { e.TitleId, e.Ordering }, "uq_title_ordering").IsUnique();

            entity.Property(e => e.AkaId).HasColumnName("aka_id");
            entity.Property(e => e.Attributes).HasColumnName("attributes");
            entity.Property(e => e.IsOriginalTitle).HasColumnName("is_original_title");
            entity.Property(e => e.Language)
                .HasMaxLength(10)
                .HasColumnName("language");
            entity.Property(e => e.LocalizedTitle).HasColumnName("localized_title");
            entity.Property(e => e.Ordering).HasColumnName("ordering");
            entity.Property(e => e.Region)
                .HasMaxLength(10)
                .HasColumnName("region");
            entity.Property(e => e.TitleId)
                .HasMaxLength(50)
                .HasColumnName("title_id");
            entity.Property(e => e.Types).HasColumnName("types");

            entity.HasOne(d => d.Title).WithMany(p => p.TitleAkas)
                .HasForeignKey(d => d.TitleId)
                .HasConstraintName("title_akas_title_id_fkey");
        });

        modelBuilder.Entity<TitleEpisode>(entity =>
        {
            entity.HasKey(e => e.EpisodeId).HasName("title_episodes_pkey");

            entity.ToTable("title_episodes");

            entity.Property(e => e.EpisodeId)
                .HasMaxLength(50)
                .HasColumnName("episode_id");
            entity.Property(e => e.EpisodeNumber).HasColumnName("episode_number");
            entity.Property(e => e.ParentTitleId)
                .HasMaxLength(50)
                .HasColumnName("parent_title_id");
            entity.Property(e => e.SeasonNumber).HasColumnName("season_number");
        });

        modelBuilder.Entity<TitlePrincipal>(entity =>
        {
            entity.HasKey(e => e.PrincipalId).HasName("title_principals_pkey");

            entity.ToTable("title_principals");

            entity.Property(e => e.PrincipalId).HasColumnName("principal_id");
            entity.Property(e => e.ActorId)
                .HasMaxLength(50)
                .HasColumnName("actor_id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.Characters).HasColumnName("characters");
            entity.Property(e => e.Job).HasColumnName("job");
            entity.Property(e => e.Ordering).HasColumnName("ordering");
            entity.Property(e => e.TitleId)
                .HasMaxLength(50)
                .HasColumnName("title_id");
        });

        modelBuilder.Entity<TitleRating>(entity =>
        {
            entity.HasKey(e => e.TitleId).HasName("title_ratings_pkey");

            entity.ToTable("title_ratings");

            entity.Property(e => e.TitleId)
                .HasMaxLength(50)
                .HasColumnName("title_id");
            entity.Property(e => e.AverageRating).HasColumnName("average_rating");
            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_updated_at");
            entity.Property(e => e.NumVotes).HasColumnName("num_votes");

            entity.HasOne(d => d.Title).WithOne(p => p.TitleRating)
                .HasForeignKey<TitleRating>(d => d.TitleId)
                .HasConstraintName("title_ratings_title_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserActivityHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("user_activity_history_pkey");

            entity.ToTable("user_activity_history");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.ActionType)
                .HasMaxLength(255)
                .HasColumnName("action_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<UserBookmark>(entity =>
        {
            entity.HasKey(e => e.BookmarkId).HasName("user_bookmarks_pkey");

            entity.ToTable("user_bookmarks");

            entity.Property(e => e.BookmarkId).HasColumnName("bookmark_id");
            entity.Property(e => e.BookmarkedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("bookmarked_at");
            entity.Property(e => e.TitleId)
                .HasMaxLength(50)
                .HasColumnName("title_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserBookmarks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_bookmarks_user_id_fkey");
        });

        modelBuilder.Entity<UserNote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("user_notes_pkey");

            entity.ToTable("user_notes");

            entity.Property(e => e.NoteId).HasColumnName("note_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasMaxLength(50)
                .HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserNotes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_notes_user_id_fkey");
        });

        modelBuilder.Entity<UserRatingHistory>(entity =>
        {
            entity.HasKey(e => e.RatingHistoryId).HasName("user_rating_history_pkey");

            entity.ToTable("user_rating_history");

            entity.Property(e => e.RatingHistoryId).HasColumnName("rating_history_id");
            entity.Property(e => e.RatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("rated_at");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.PreviousRating).HasColumnName("previous_rating");
            entity.Property(e => e.TitleId)
                .HasMaxLength(50)
                .HasColumnName("title_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<WordIndex>(entity =>
        {
            entity.HasKey(e => e.WordId).HasName("word_index_pkey");

            entity.ToTable("word_index");

            entity.Property(e => e.WordId).HasColumnName("word_id");
            entity.Property(e => e.Word).HasColumnName("word");
        });

        modelBuilder.Entity<WordIndexTitle>(entity =>
        {
            entity.HasKey(e => e.WiId).HasName("word_index_titles_pkey");

            entity.ToTable("word_index_titles");

            entity.Property(e => e.WiId).HasColumnName("wi_id");
            entity.Property(e => e.SourceField)
                .HasMaxLength(1)
                .HasColumnName("source_field");
            entity.Property(e => e.TermFreq).HasColumnName("term_freq");
            entity.Property(e => e.TitleId)
                .HasMaxLength(50)
                .HasColumnName("title_id");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.WordId).HasColumnName("word_id");

            entity.HasOne(d => d.Title).WithMany(p => p.WordIndexTitles)
                .HasForeignKey(d => d.TitleId)
                .HasConstraintName("word_index_titles_title_id_fkey");

            entity.HasOne(d => d.Word).WithMany(p => p.WordIndexTitles)
                .HasForeignKey(d => d.WordId)
                .HasConstraintName("word_index_titles_word_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
