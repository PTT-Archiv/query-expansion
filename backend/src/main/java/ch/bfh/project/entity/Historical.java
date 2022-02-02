package ch.bfh.project.entity;

import lombok.Getter;
import lombok.Setter;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "historical")
@Getter
@Setter
public class Historical {

	@Id
	@Column(nullable = false)
	private int id;
	private int fk_localitiesid;
	private String names;
}
