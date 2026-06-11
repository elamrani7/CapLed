import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { BadgeDisponibilite, BadgeCondition } from '../shared/Badge';
import { resolveAssetUrl } from '../../api/assets';

export const ProductCardSkeleton = () => (
  <div className="pf-product-card pf-product-card-skeleton" aria-hidden="true">
    <div className="pf-skeleton-media"></div>
    <div className="pf-skeleton-body">
      <span className="pf-skeleton-line short"></span>
      <span className="pf-skeleton-line title"></span>
      <span className="pf-skeleton-line"></span>
      <span className="pf-skeleton-line"></span>
      <span className="pf-skeleton-button"></span>
    </div>
  </div>
);

const ProductFallbackVisual = ({ product }: { product: any }) => (
  <div className="pf-product-fallback">
    <div className="pf-product-fallback-mark">
      <i className="bi bi-cpu"></i>
    </div>
    <div className="pf-product-fallback-brand">PartFinder</div>
    <div className="pf-product-fallback-ref">{product.reference || 'Produit industriel'}</div>
  </div>
);

export const ProductCard = ({ product }: { product: any }) => {
  const [imageFailed, setImageFailed] = useState(false);
  const imageUrl = resolveAssetUrl(product.urlImagePrincipale);
  const hasImage = !!imageUrl && !imageFailed;
  const hasDispoInfo = product.disponibiliteBadge &&
    product.disponibiliteBadge.toUpperCase() !== 'NON_SPECIFIE' &&
    product.disponibiliteBadge.toUpperCase() !== 'NON SPECIFIE' &&
    product.disponibiliteBadge.toUpperCase() !== 'NON SPÉCIFIÉ';
  const hasCondition = product.condition &&
    product.condition.toUpperCase() !== 'NON_SPECIFIE';

  return (
    <article className="pf-product-card">
      <div className="pf-product-image-wrap">
        {hasImage ? (
          <img
            src={imageUrl}
            alt={product.nom}
            onError={() => setImageFailed(true)}
            style={{ width: '100%', height: '100%', objectFit: 'contain', padding: '12px' }}
          />
        ) : (
          <ProductFallbackVisual product={product} />
        )}

        {hasDispoInfo && (
          <div className="pf-product-badge pf-product-badge-left">
            <BadgeDisponibilite dispo={product.disponibiliteBadge} />
          </div>
        )}
        {hasCondition && (
          <div className="pf-product-badge pf-product-badge-right">
            <BadgeCondition condition={product.condition} grade={product.gradeVisuel} />
          </div>
        )}
      </div>

      <div className="pf-product-body">
        <div className="pf-product-ref">Ref: {product.reference || '-'}</div>
        <h3 className="pf-product-title mb-0">{product.nom}</h3>

        {product.marque && (
          <div>
            <span className="pf-badge-marque">{product.marque}</span>
          </div>
        )}

        {product.caracteristiquesPrincipales?.length > 0 && (
          <ul className="pf-product-specs">
            {product.caracteristiquesPrincipales.slice(0, 2).map((feat: any, idx: number) => (
              <li key={idx}>
                <span>{feat.nom}</span>
                <strong>{feat.valeur}</strong>
              </li>
            ))}
          </ul>
        )}

        <div className="pf-product-footer">
          <div className="pf-product-price-note">
            <i className="bi bi-tag me-1"></i>Prix sur devis
          </div>
          <Link to={`/catalogue/${product.id}`} className="pf-btn-devis">
            <span>Demander un devis</span>
            <i className="bi bi-arrow-right"></i>
          </Link>
        </div>
      </div>
    </article>
  );
};
