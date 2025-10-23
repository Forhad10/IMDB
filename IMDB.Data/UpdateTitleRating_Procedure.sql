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
