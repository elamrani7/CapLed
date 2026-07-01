import React from 'react';
import { Badge } from '../shared/Badge';

type EtatDetail = {
  gradeVisuel?: string;
  pannesObservees?: string;
  testsFonctionnels?: string;
  revisionsEffectuees?: string;
  garantieOfferte?: number;
};

export const EtatDetailCard = ({ etatDetail }: { etatDetail: EtatDetail }) => {
  if (!etatDetail) return null;

  return (
    <div className="card shadow-sm border-info">
      <div className="card-header bg-info text-white d-flex align-items-center justify-content-between">
        <h5 className="mb-0 fw-bold"><i className="bi bi-clipboard2-check me-2"></i> Rapport de Condition</h5>
        <div className="d-flex gap-2">
          {etatDetail.gradeVisuel && (
            <span className="badge bg-light text-info fw-bold border border-info">
              Grade {etatDetail.gradeVisuel}
            </span>
          )}
          {etatDetail.garantieOfferte !== undefined && etatDetail.garantieOfferte > 0 && (
            <span className="badge bg-success fw-bold">
              Garantie {etatDetail.garantieOfferte} mois
            </span>
          )}
        </div>
      </div>

      <div className="accordion accordion-flush" id="etatAccordion">
        {etatDetail.testsFonctionnels && (
          <div className="accordion-item">
            <h2 className="accordion-header">
              <button className="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseTests" aria-expanded="true">
                <i className="bi bi-check-circle text-success me-2"></i> Tests Fonctionnels Réalisés
              </button>
            </h2>
            <div id="collapseTests" className="accordion-collapse collapse show">
              <div className="accordion-body bg-light border-start border-success border-4 ms-3 my-2" style={{ whiteSpace: 'pre-wrap' }}>
                {etatDetail.testsFonctionnels}
              </div>
            </div>
          </div>
        )}

        {etatDetail.revisionsEffectuees && (
          <div className="accordion-item">
            <h2 className="accordion-header">
              <button className="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseRevs">
                <i className="bi bi-tools text-primary me-2"></i> Révisions et Remplacements
              </button>
            </h2>
            <div id="collapseRevs" className="accordion-collapse collapse">
              <div className="accordion-body bg-light border-start border-primary border-4 ms-3 my-2" style={{ whiteSpace: 'pre-wrap' }}>
                {etatDetail.revisionsEffectuees}
              </div>
            </div>
          </div>
        )}

        {etatDetail.pannesObservees && (
          <div className="accordion-item">
            <h2 className="accordion-header">
              <button className="accordion-button collapsed text-danger" type="button" data-bs-toggle="collapse" data-bs-target="#collapsePannes">
                <i className="bi bi-exclamation-triangle text-danger me-2"></i> Anomalies Connues (Non-impactantes)
              </button>
            </h2>
            <div id="collapsePannes" className="accordion-collapse collapse">
              <div className="accordion-body bg-light border-start border-danger border-4 ms-3 my-2 text-danger" style={{ whiteSpace: 'pre-wrap' }}>
                {etatDetail.pannesObservees}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
