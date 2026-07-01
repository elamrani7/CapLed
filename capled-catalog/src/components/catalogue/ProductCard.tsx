import React, { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { BadgeDisponibilite, BadgeCondition } from '../shared/Badge';
import { resolveProductImageUrls } from '../../api/assets';
import { catalogueApi } from '../../api/catalogueApi';

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
  const initialImageUrls = useMemo(() => resolveProductImageUrls(product), [product]);
  const [imageUrls, setImageUrls] = useState<string[]>(initialImageUrls);
  const [imageIndex, setImageIndex] = useState(0);
  const [detailFallbackLoaded, setDetailFallbackLoaded] = useState(false);
  const imageUrl = imageUrls[imageIndex];
  const hasImage = !!imageUrl;
  const hasDispoInfo = product.disponibiliteBadge &&
    product.disponibiliteBadge.toUpperCase() !== 'NON_SPECIFIE' &&
    product.disponibiliteBadge.toUpperCase() !== 'NON SPECIFIE' &&
    product.disponibiliteBadge.toUpperCase() !== 'NON SPÉCIFIÉ';
  const hasCondition = product.condition &&
    product.condition.toUpperCase() !== 'NON_SPECIFIE';

  const handleImageError = async () => {
    if (imageIndex < imageUrls.length - 1) {
      setImageIndex((current) => current + 1);
      return;
    }

    if (!detailFallbackLoaded && product.id) {
      setDetailFallbackLoaded(true);
      try {
        const detail = await catalogueApi.getProductById(product.id);
        const nextUrls = resolveProductImageUrls(detail);
        if (nextUrls.length > 0) {
          setImageUrls(nextUrls);
          setImageIndex(0);
        } else {
          setImageUrls([]);
        }
      } catch {
        setImageUrls([]);
      }
      return;
    }

    setImageUrls([]);
  };

  return (
    <article className="pf-product-card">
      <div className="pf-product-image-wrap">
        {hasImage ? (
          <img
            src={imageUrl}
            alt={product.nom}
            onError={handleImageError}
            className="pf-product-image"
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
            <i className="bi bi-arrow-right-short"></i>
          </Link>
        </div>
      </div>
    </article>
  );
};
