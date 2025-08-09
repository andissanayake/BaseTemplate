import { useEffect, useState, forwardRef, useImperativeHandle } from "react";
import {
  Typography,
  Table,
  Card,
  Space,
  Tag,
  notification,
  Alert,
  Button,
  InputNumber,
} from "antd";
import { EditOutlined, ReloadOutlined } from "@ant-design/icons";
import { ItemVariant } from "./ItemVariantModel";
import { apiClient } from "../../common/apiClient";

interface ItemVariantsStepProps {
  itemId: string;
  onLoadingChange: (loading: boolean) => void;
}

export interface ItemVariantsStepHandle {
  refresh: () => void;
}

const ItemVariantsStep = forwardRef<
  ItemVariantsStepHandle,
  ItemVariantsStepProps
>(({ itemId, onLoadingChange }, ref) => {
  const [variants, setVariants] = useState<ItemVariant[]>([]);
  const [loading, setLoading] = useState(false);
  const [hasVariants, setHasVariants] = useState(false);

  useImperativeHandle(ref, () => ({
    refresh: loadVariants,
  }));

  useEffect(() => {
    loadVariants();
  }, [itemId]);

  const loadVariants = () => {
    setLoading(true);
    onLoadingChange(true);

    apiClient.get<ItemVariant[]>(`/api/item/${itemId}/variants`, {
      onSuccess: (data) => {
        setVariants(data);
        setHasVariants(data.length > 0);
      },
      onFallback: () => {
        notification.error({
          message: "Error",
          description: "Failed to load item variants",
        });
      },
      onFinally: () => {
        setLoading(false);
        onLoadingChange(false);
      },
    });
  };

  const handlePriceChange = (variantId: number, newPrice: number) => {
    // This would typically call an API to update the variant price
    setVariants((prev) =>
      prev.map((variant) =>
        variant.id === variantId ? { ...variant, price: newPrice } : variant
      )
    );

    // TODO: Add API call to update variant price
    notification.success({
      message: "Price Updated",
      description: "Variant price has been updated successfully",
    });
  };

  const columns = [
    {
      title: "Variant Code",
      dataIndex: "code",
      key: "code",
      render: (code: string) => (
        <Typography.Text strong>{code}</Typography.Text>
      ),
    },
    {
      title: "Characteristics",
      key: "characteristics",
      width: "40%",
      render: (_: unknown, record: ItemVariant) => (
        <Space direction="vertical" size="small" style={{ width: "100%" }}>
          {record.characteristics.map((char) => (
            <div key={char.id} style={{ marginBottom: 4 }}>
              <Tag
                color="geekblue"
                style={{
                  fontSize: "12px",
                  fontWeight: "500",
                  padding: "4px 8px",
                  borderRadius: "6px",
                  minWidth: "fit-content",
                }}
                title={`${char.characteristicTypeName}: ${
                  char.characteristicName
                }${
                  char.characteristicValue
                    ? ` (${char.characteristicValue})`
                    : ""
                }`}
              >
                <Typography.Text strong style={{ color: "inherit" }}>
                  {char.characteristicTypeName}:
                </Typography.Text>{" "}
                {char.characteristicName}
                {char.characteristicValue && (
                  <Typography.Text
                    style={{ fontStyle: "italic", color: "inherit" }}
                  >
                    {" "}
                    ({char.characteristicValue})
                  </Typography.Text>
                )}
              </Tag>
            </div>
          ))}
        </Space>
      ),
    },
    {
      title: "Price",
      dataIndex: "price",
      key: "price",
      render: (price: number, record: ItemVariant) => (
        <InputNumber
          value={price}
          min={0}
          step={0.01}
          precision={2}
          formatter={(value) =>
            `$ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",")
          }
          parser={(value) => Number(value!.replace(/\$\s?|(,*)/g, ""))}
          onChange={(value) =>
            value !== null && handlePriceChange(record.id, value)
          }
          style={{ width: "120px" }}
        />
      ),
    },
    {
      title: "Actions",
      key: "actions",
      render: (_: unknown, record: ItemVariant) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => {
              // TODO: Navigate to variant edit page or open modal
              notification.info({
                message: "Edit Variant",
                description: `Edit functionality for variant ${record.code} coming soon`,
              });
            }}
          >
            Edit
          </Button>
        </Space>
      ),
    },
  ];

  if (!hasVariants && !loading) {
    return (
      <Card>
        <Alert
          message="No Variants"
          description="This item doesn't have any variants yet. Variants are automatically generated based on the characteristic types assigned to the item."
          type="info"
          showIcon
          action={
            <Button
              size="small"
              icon={<ReloadOutlined />}
              onClick={loadVariants}
            >
              Refresh
            </Button>
          }
        />
      </Card>
    );
  }

  return (
    <Card>
      <Space direction="vertical" size="large" style={{ width: "100%" }}>
        <div
          style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <div>
            <Typography.Title level={4} style={{ margin: 0 }}>
              Item Variants ({variants.length})
            </Typography.Title>
            <Typography.Text type="secondary">
              Manage variants and their pricing
            </Typography.Text>
          </div>
          <Button
            icon={<ReloadOutlined />}
            onClick={loadVariants}
            loading={loading}
          >
            Refresh
          </Button>
        </div>

        <Table
          columns={columns}
          dataSource={variants}
          rowKey="id"
          loading={loading}
          pagination={{
            pageSize: 10,
            showSizeChanger: true,
            showQuickJumper: true,
            showTotal: (total, range) =>
              `${range[0]}-${range[1]} of ${total} variants`,
          }}
          scroll={{ x: true }}
        />
      </Space>
    </Card>
  );
});

ItemVariantsStep.displayName = "ItemVariantsStep";

export { ItemVariantsStep };
