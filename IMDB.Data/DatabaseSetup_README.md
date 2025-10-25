# Database Setup Instructions

## PostgreSQL Stored Procedure Setup

To use the optimized `UpdateTitleRating` stored procedure, you need to execute it in your PostgreSQL database.

### 1. Connect to PostgreSQL Database

```bash
psql -h localhost -d IMDB -U postgres -W
```

### 2. Execute the Stored Procedure

Run the following SQL script to create the stored procedure:

```sql
-- Final stored procedure to update title ratings based on user rating actions
-- Parameters: titleId, userId, and action ('update' or 'remove')
CREATE OR REPLACE FUNCTION UpdateTitleRating(
    p_title_id VARCHAR,
    p_user_id UUID,
    p_action VARCHAR -- 'update' or 'remove'
)
RETURNS VOID
LANGUAGE plpgsql
AS $$
DECLARE
    v_user_rating NUMERIC := 0;
    v_old_avg DOUBLE PRECISION := 0;
    v_old_votes INTEGER := 0;
    v_new_avg DOUBLE PRECISION := 0;
    v_new_votes INTEGER := 0;
BEGIN
    -- Step 1: Get user rating
    SELECT COALESCE(rating, 0)
    INTO v_user_rating
    FROM user_rating_histories
    WHERE title_id = p_title_id AND user_id = p_user_id
    LIMIT 1;

    IF v_user_rating = 0 THEN
        RAISE NOTICE '❌ User rating not found for % and %', p_user_id, p_title_id;
        RETURN;
    END IF;

    -- Step 2: Get current average and votes from title_ratings
    SELECT
        COALESCE(average_rating, 0),
        COALESCE(num_votes, 0)
    INTO v_old_avg, v_old_votes
    FROM title_ratings
    WHERE title_id = p_title_id;

    -- Step 3: Calculate based on action
    IF LOWER(p_action) = 'update' THEN
        v_new_votes := v_old_votes + 1;
        v_new_avg := ((v_old_avg * v_old_votes) + v_user_rating) / v_new_votes;
    ELSIF LOWER(p_action) = 'remove' THEN
        IF v_old_votes <= 1 THEN
            v_new_votes := 0;
            v_new_avg := 0;
        ELSE
            v_new_votes := v_old_votes - 1;
            v_new_avg := ((v_old_avg * v_old_votes) - v_user_rating) / v_new_votes;
        END IF;
    ELSE
        RAISE NOTICE '❌ Invalid action: %', p_action;
        RETURN;
    END IF;

    -- Step 4: Insert/update title_ratings
    INSERT INTO title_ratings (title_id, average_rating, num_votes, last_updated_at)
    VALUES (p_title_id, v_new_avg, v_new_votes, NOW())
    ON CONFLICT (title_id)
    DO UPDATE SET
        average_rating = EXCLUDED.average_rating,
        num_votes = EXCLUDED.num_votes,
        last_updated_at = EXCLUDED.last_updated_at;

    -- Step 5: Log message
    RAISE NOTICE '✅ Action=%: title_id=%, user_id=%, user_rating=%, new_avg=%, new_votes=%',
                 p_action, p_title_id, p_user_id, v_user_rating, v_new_avg, v_new_votes;
END;
$$;
```

### 3. Alternative: Using the SQL File

You can also execute the stored procedure directly from the file:

```bash
psql -h localhost -d IMDB -U postgres -W -f IMDB.Data/UpdateTitleRating_Procedure.sql
```

### 4. Verify Installation

Test that the stored procedure was created successfully:

```sql
-- Check if function exists
\df UpdateTitleRating

-- Test the procedure (replace 'tt0111161' with an actual title_id and 'user-uuid' with actual UUID)
-- For UPDATE action (adding/updating a rating)
SELECT UpdateTitleRating('tt0111161', '12345678-1234-1234-1234-123456789abc'::UUID, 'update');

-- For REMOVE action (removing a rating)
SELECT UpdateTitleRating('tt0111161', '12345678-1234-1234-1234-123456789abc'::UUID, 'remove');

-- Check the updated title_ratings table
SELECT * FROM title_ratings WHERE title_id = 'tt0111161';
```

## Benefits of Using Stored Procedure

1. **Performance**: Database calculations are faster than application-level calculations
2. **Consistency**: All rating calculations happen in one place
3. **Atomic Operations**: Uses `ON CONFLICT` for safe upsert operations
4. **Maintainability**: Easy to modify calculation logic without changing application code

## Usage in Application

The stored procedure is automatically called by:
- `AddOrUpdateRatingAsync(titleId, userId, "update")` - When adding or updating user ratings
- `RemoveRatingAsync(userId, titleId, "remove")` - When removing user ratings

The stored procedure accepts three parameters:
- **titleId** (required): The movie/TV show identifier
- **userId** (required): The specific user's UUID
- **action** (required): Either 'update' or 'remove'

The procedure performs **incremental calculation**:
- **For 'update'**: Gets current average from `title_ratings`, adds the user's rating, and recalculates
- **For 'remove'**: Gets current average from `title_ratings`, removes the user's rating, and recalculates
- **Efficient**: No need to scan all user ratings each time, uses existing aggregated data

## How Incremental Calculation Works

```
For UPDATE action:
NewVotes = OldVotes + 1
NewAverage = ((OldAverage × OldVotes) + UserRating) / NewVotes

For REMOVE action:
NewVotes = OldVotes - 1
NewAverage = ((OldAverage × OldVotes) - UserRating) / NewVotes
```

No additional code changes are needed in the application controllers.

## Database Schema Changes

When adding new columns to your database, you need to update the Entity Framework model. Here's what was done for the `previous_rating` column:

### **Step 1: Database Change**
```sql
ALTER TABLE user_rating_histories ADD COLUMN previous_rating smallint;
```

### **Step 2: Entity Class Update**
Added property to `UserRatingHistory.cs`:
```csharp
public short? PreviousRating { get; set; }
```

### **Step 3: DbContext Configuration**
Added property mapping in `IMDBDbContext.cs`:
```csharp
entity.Property(e => e.PreviousRating).HasColumnName("previous_rating");
```

### **Step 4: DTO Update**
Added field to `UserRatingHistoryDto`:
```csharp
public short? PreviousRating { get; set; }
```

### **Step 5: Service Logic Update**
- Updated SQL queries to include `PreviousRating`
- Modified rating update logic to store previous values
- Added proper error handling with try-catch blocks

## Complete Guide: Adding New Database Columns

### **1. Database Level**
```sql
ALTER TABLE table_name ADD COLUMN column_name data_type;
```

### **2. Entity Framework Level**
- Add property to entity class
- Add property configuration in DbContext
- Update DTOs if needed
- Update service methods

### **3. Testing**
- Verify database operations work
- Check API responses include new field
- Test edge cases and error scenarios

The `previous_rating` field tracks the user's previous rating value, allowing you to see rating change history and implement features like "rating changed from X to Y".
