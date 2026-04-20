import React from 'react';
import { Link } from 'react-router-dom';
import { BadgeDisponibilite, BadgeCondition } from '../shared/Badge';

export const ProductCard = ({ product }: { product: any }) => {

  return (
    <div className="card h-100 bg-white autodoc-card rounded-0 border-0 shadow-sm" style={{ outline: '1px solid #e9ecef' }}>
      {/* Container de l'image (Plus compact) */}
      <div className="position-relative bg-white d-flex align-items-center justify-content-center overflow-hidden p-2" style={{ height: '140px', borderBottom: '1px solid #f0f0f0' }}>
        {product.urlImagePrincipale ? (
          <img 
            src={`http://localhost:5000${product.urlImagePrincipale}`} 
            alt={product.nom} 
            className="w-100 h-100 object-fit-contain"
          />
        ) : (
          <div className="text-muted small fw-medium d-flex flex-column align-items-center">
            <i className="bi bi-image fs-3 text-light"></i>
            <span style={{ fontSize: '0.7rem' }}>Aucune image</span>
          </div>
        )}
        
        {/* Badges Overlay (Compact) */}
        <div className="position-absolute top-0 start-0 p-1 d-flex flex-column gap-1" style={{ scale: '0.85', transformOrigin: 'top left' }}>
          <BadgeDisponibilite dispo={product.disponibiliteBadge} />
        </div>
        <div className="position-absolute top-0 end-0 p-1 d-flex flex-column gap-1" style={{ scale: '0.85', transformOrigin: 'top right' }}>
          <BadgeCondition condition={product.condition} grade={product.gradeVisuel} />
        </div>
      </div>

      {/* Détails du produit (Denses) */}
      <div className="card-body d-flex flex-column p-2 pt-3">
        <div className="text-secondary mb-1 text-uppercase font-monospace" style={{ fontSize: '0.7rem' }}>Ref: {product.reference}</div>
        <h6 className="card-title fw-bold text-dark lh-sm mb-2" style={{ fontSize: '0.9rem', display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }} title={product.nom}>
          {product.nom}
        </h6>
        
        {/* EAV Résumé (Très minimaliste) */}
        <div className="flex-grow-1">
          {product.caracteristiquesPrincipales && product.caracteristiquesPrincipales.length > 0 && (
            <ul className="list-unstyled mb-2 d-flex flex-column">
              {product.caracteristiquesPrincipales.slice(0, 3).map((feat: any, idx: number) => (
                <li key={idx} className="text-truncate text-muted d-flex justify-content-between border-bottom border-light pb-1 mb-1" style={{ fontSize: '0.75rem' }}>
                  <span>{feat.nom}</span>
                  <span className="fw-bold text-dark">{feat.valeur}</span>
                </li>
              ))}
            </ul>
          )}
        </div>

        {/* Footer info (CTA) */}
        <div className="mt-2 text-end">
          <div className="mb-2 d-flex align-items-baseline justify-content-end gap-1">
            <span className="fs-6 fw-bold text-primary lh-1"><i className="bi bi-tag me-1"></i>Prix sur devis</span>
          </div>
          <Link 
            to={`/catalogue/${product.id}`}
            className="btn btn-warning w-100 fw-bold border-dark border-opacity-10 py-1"
            style={{ fontSize: '0.85rem' }}
          >
            Demander un devis
          </Link>
        </div>
      </div>
    </div>
  );
};
