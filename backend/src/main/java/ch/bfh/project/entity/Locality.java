package ch.bfh.project.entity;

import lombok.Getter;
import lombok.Setter;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import java.util.Objects;

@Entity
@Table(name = "localities")
@Getter
@Setter
public class Locality {

	@Id
	@Column(nullable = false)
	private int id;
	private String de;
	private String fr;
	private String it;

	@Override
	public boolean equals(Object o) {
		if (this == o) return true;
		if (o == null || getClass() != o.getClass()) return false;
		Locality locality = (Locality) o;
		return Objects.equals(de, locality.de) &&
				Objects.equals(fr, locality.fr) &&
				Objects.equals(it, locality.it);
	}

	@Override
	public int hashCode() {
		return Objects.hash(de, fr, it);
	}
}
