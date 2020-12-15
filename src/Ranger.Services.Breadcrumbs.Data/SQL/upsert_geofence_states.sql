DROP FUNCTION IF EXISTS public.upsert_device_geofence_states(TEXT, UUID, TEXT, UUID[], TIMESTAMP WITHOUT TIME ZONE);

CREATE OR REPLACE FUNCTION public.upsert_device_geofence_states(v_tenant_id TEXT, v_project_id UUID, v_device_id TEXT, v_geofence_ids UUID[], v_recorded_at TIMESTAMP WITHOUT TIME ZONE)
RETURNS TABLE(o_project_id UUID, o_device_id TEXT, o_geofence_id UUID, o_last_event INT) AS
$$
DECLARE
	g_id UUID;
BEGIN
    -- lock the rows until this breadcrumb is finished
 	PERFORM pg_advisory_xact_lock(q.id) FROM
 	(
 		SELECT id FROM device_geofence_states
 		WHERE project_id = v_project_id
 		AND device_id = v_device_id
 	) q;
	
    -- check if this breadcrumb is newer, discard redundant and outdated breadcrumbs
 	IF EXISTS (SELECT 1 FROM last_device_recorded_ats 
			   WHERE project_id = v_project_id 
			   AND device_id = v_device_id 
			   AND recorded_at >= v_recorded_at FOR UPDATE)
 	THEN
 		-- breadcrumb is outdated
 		RAISE SQLSTATE '50001';
 	ELSE
        -- store this as the latest breadcrumb
 		INSERT INTO last_device_recorded_ats(tenant_id, project_id, device_id, recorded_at) VALUES (v_tenant_id, v_project_id, v_device_id, v_recorded_at)
 		ON CONFLICT (project_id, device_id)
		DO UPDATE SET recorded_at = v_recorded_at;
 	END IF;
	
    -- upsert the geofences the user is in
 	FOREACH g_id IN ARRAY v_geofence_ids
 	LOOP
 		INSERT INTO device_geofence_states(tenant_id, project_id, device_id, geofence_id, recorded_at, last_event) VALUES (v_tenant_id, v_project_id, v_device_id, g_id, v_recorded_at, 1)
 		ON CONFLICT (project_id, geofence_id, device_id)
		DO UPDATE SET recorded_at = v_recorded_at, last_event = 2;
 	END LOOP;
	
    -- store the result in a transactional temp table
	CREATE TEMP TABLE results ON COMMIT DROP AS
	SELECT 
		dgs.project_id, 
		dgs.device_id, 
		dgs.geofence_id,
		CASE WHEN dgs.recorded_at < ldr.recorded_at THEN 3
			 ELSE dgs.last_event
		END AS last_event
	FROM device_geofence_states dgs
	INNER JOIN last_device_recorded_ats ldr
	ON dgs.project_id = ldr.project_id
	AND dgs.device_id = ldr.device_id
	WHERE ldr.project_id = v_project_id
	AND ldr.device_id = v_device_id;	
		
    -- delete 'exited' geofences
	DELETE 
	FROM device_geofence_states
	WHERE project_id = v_project_id
	AND device_id = v_device_id
	AND recorded_at < v_recorded_at;
 	
 	RETURN QUERY
	SELECT * FROM results;
END;
$$
LANGUAGE plpgsql;

ALTER FUNCTION public.upsert_device_geofence_states(TEXT, UUID, TEXT, UUID[], TIMESTAMP WITHOUT TIME ZONE) OWNER TO postgres;