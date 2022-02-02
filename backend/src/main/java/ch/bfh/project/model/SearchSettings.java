package ch.bfh.project.model;

import lombok.Data;

@Data
public class SearchSettings {
	/**
	 * Indicates whether the search should be expanded with the translation of localities
	 */
	private final boolean translatedLocalityNames;

	/**
	 * Indicates whether the search should be expanded with historical names of localities
	 */
	private final boolean historicalLocalityNames;
}
