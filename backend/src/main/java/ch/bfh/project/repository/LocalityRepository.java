package ch.bfh.project.repository;

import ch.bfh.project.entity.Locality;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.Set;

@Repository
public interface LocalityRepository extends JpaRepository<Locality, Long> {

	/*
	 * A locality can be contained several times in the database, which is why we return a set of localities and
	 * not a single locality
	 */
	@Query(value = "select l from Locality l where l.de = :searchTerm or l.fr = :searchTerm or l.it = :searchTerm")
	Set<Locality> findLocalitiesByKeyword(String searchTerm);

}
