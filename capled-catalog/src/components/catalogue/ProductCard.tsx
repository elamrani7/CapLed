import React from 'react';
import { Link } from 'react-router-dom';
import { BadgeDisponibilite, BadgeCondition } from '../shared/Badge';

export const ProductCard = ({ product }: { product: any }) => {
  const hasImage = !!product.urlImagePrincipale;
  const hasDispoInfo = product.disponibiliteBadge &&
    product.disponibiliteBadge.toUpperCase() !== 'NON_SPECIFIE' &&
    product.disponibiliteBadge.toUpperCase() !== 'NON SPÉCIFIÉ';
  const hasCondition = product.condition &&
    product.condition.toUpperCase() !== 'NON_SPECIFIE';

  return (
    <div className="pf-product-card">
      {/* Image */}
      <div className="pf-product-image-wrap">
        {hasImage ? (
          <img
            src={`https://capled-api.onrender.com${product.urlImagePrincipale}`}
            alt={product.nom}
            style={{ width: '100%', height: '100%', objectFit: 'contain', padding: '12px' }}
          />
        ) : (
          <div className="d-flex flex-column align-items-center" style={{ color: 'var(--pf-gray-400)' }}>
            <i className="bi bi-image" style={{ fontSize: '2rem', opacity: 0.25 }}></i>
            <span style={{ fontSize: '0.7rem', marginTop: '4px', opacity: 0.5 }}>Aucune image</span>
          </div>
        )}

        {/* Badges — only show if meaningful */}
        {hasDispoInfo && (
          <div style={{ position: 'absolute', top: 8, left: 8 }}>
            <BadgeDisponibilite dispo={product.disponibiliteBadge} />
          </div>
        )}
        {hasCondition && (
          <div style={{ position: 'absolute', top: 8, right: 8 }}>
            <BadgeCondition condition={product.condition} grade={product.gradeVisuel} />
          </div>
        )}
      </div>

      {/* Body */}
      <div className="d-flex flex-column p-3 flex-grow-1" style={{ gap: '6px' }}>
        {/* Reference */}
        <div className="pf-product-ref">Réf: {product.reference || '—'}</div>

        {/* Title */}
        <h3 className="pf-product-title mb-0">{product.nom}</h3>

        {/* Brand */}
        {product.marque && (
          <div>
            <span className="pf-badge-marque">{product.marque}</span>
          </div>
        )}

        {/* Key specs */}
        {product.caracteristiquesPrincipales?.length > 0 && (
          <ul className="list-unstyled mb-0 flex-grow-1" style={{ marginTop: '4px' }}>
            {product.caracteristiquesPrincipales.slice(0, 2).map((feat: any, idx: number) => (
              <li key={idx}
                className="d-flex justify-content-between"
                style={{
                  fontSize: '0.72rem', color: 'var(--pf-gray-600)',
                  borderBottom: '1px solid var(--pf-border)',
                  paddingBottom: '3px', marginBottom: '3px'
                }}>
                <span>{feat.nom}</span>
                <span style={{ fontWeight: 600, color: 'var(--pf-gray-800)' }}>{feat.valeur}</span>
              </li>
            ))}
          </ul>
        )}

        {/* Spacer */}
        <div className="flex-grow-1" style={{ minHeight: '8px' }}></div>

        {/* Price indicator + CTA */}
        <div style={{ marginTop: '8px' }}>
          <div style={{ fontSize: '0.75rem', color: 'var(--pf-gray-400)', marginBottom: '8px', fontWeight: 500 }}>
            <i className="bi bi-tag me-1"></i>Prix sur devis
          </div>
          <Link to={`/catalogue/${product.id}`} className="pf-btn-devis">
            <i className="bi bi-cart-plus"></i>Demander un devis
          </Link>
        </div>
      </div>
    </div>
  );
};
