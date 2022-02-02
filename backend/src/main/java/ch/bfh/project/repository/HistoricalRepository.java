package ch.bfh.project.repository;

import ch.bfh.project.entity.Historical;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.Set;

@Repository
public interface HistoricalRepository extends JpaRepository<Historical, Long> {

	@Query(value = "select h from Historical h where h.fk_localitiesid in :localitiesId")
	Set<Historical> findByLocalitiesIds(Set<Integer> localitiesId);
}
